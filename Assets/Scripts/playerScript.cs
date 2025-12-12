using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(AudioSource))]
public class playerScript : MonoBehaviour
{


    public AudioClip jumpSound;
    public AudioClip runSound;

    AudioSource _audioSouce;
    public float maxMoveSpeed = 7f;
    public float groundAcceleration = 50f;
    public float airAcceleration = 20f;
    public float groundDeceleration = 40f;
    public float airDeceleration = 10f;

    public float sprintingMultiplier = 2f;
    public float crouchingMultiplier = 0.5f;

    public Vector2 standingCollider = new Vector2(1.6f, 3.4f);
    public Vector2 crouchingCollider = new Vector2(2.2f, 2.27f);

    public float jumpForce = 600f;
    private enum jumpState
    {
        Walking,
        Sprinting,
        Crouching,
        NoJump
    }
    private jumpState currentJumpState;
    private float lastGrounded;
    private float lastJump;
    private float lastWallJump;
    private bool tryStandUp;
    private bool isSprinting;
    private bool isCrouching;
    private bool isWalking;
    private bool isJumping;
    private float moveDirection = 0;
    private LayerMask groundLayers;
    private LayerMask guardLayer;
    private Rigidbody2D rbody;
    private Animator animator;
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;
    private bool facingRight = true;

    void Start()
    {
        _audioSouce = GetComponent<AudioSource>();
        rbody = GetComponent<Rigidbody2D>();
        groundLayers = LayerMask.GetMask("Platform", "Solid Platform");
        guardLayer = LayerMask.GetMask("Guard");
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        lastGrounded = 0f;
        lastJump = 0f;
        tryStandUp = false;
        isSprinting = false;
        isCrouching = false;
        isWalking = false;
        isJumping = false;
        moveDirection = 0f;
        lastWallJump = 0f;
        currentJumpState = jumpState.NoJump;
    }

    void Update()
    {

    }

    private void FixedUpdate()
    {
        bool onGround = OnGround();
        isWalking = Mathf.Abs(rbody.linearVelocityX) > 0.05f;
        isJumping = !onGround;

        // Set animator booleans
        animator.SetBool("Walking", isWalking);
        animator.SetBool("Jumping", isJumping);
        animator.SetBool("Crouching", isCrouching);
        if (PlayerPrefs.GetInt("PlayerImmune") == 0)
        {
            if (rbody.linearVelocityX < -0.05f)
            {
                spriteRenderer.flipX = true;
                facingRight = false;
            }
            else if (rbody.linearVelocityX > 0.05f)
            {
                spriteRenderer.flipX = false;
                facingRight = true;
            }


            // Set last grounded for jumping timings
            if (onGround)
            {
                lastGrounded = Time.time;
                if (Time.time - lastJump > 0.1f)
                    currentJumpState = jumpState.NoJump;

                // If just landed after a wall jump, cancel the cooldown and reset horizontal velocity
                if (Time.time - lastWallJump < 0.4f)
                {
                    rbody.linearVelocityX = 0;
                    lastWallJump = 0;
                }
            }

            // If player tries to stand up but can't, keep trying until they can
            if (tryStandUp)
            {
                if (CanStandUp())
                {
                    StandUp();
                    tryStandUp = false;
                }
            }

            // Block movement during wall jump cooldown
            if (Time.time - lastWallJump < 0.4f)
            {
                return;
            }

            // Handle movement with acceleration
            HandleMovement(onGround);
        }
        else {
            moveDirection = 0;
            rbody.linearVelocityX = 0;
            rbody.linearVelocityY = 0;
        }
    }

    private void HandleMovement(bool onGround)
    {
        if (moveDirection < 0 && !CanGoLeft())
        {
            return;
        }
        else if (moveDirection > 0 && !CanGoRight())
        {
            return;
        }
        // Calculate target speed based on modifiers
        float speedMultiplier = 1f;
        if (onGround)
        {
            if (isCrouching)
            {
                speedMultiplier = crouchingMultiplier;
            }
            else if (isSprinting)
            {
                speedMultiplier = sprintingMultiplier;
            }
        }
        else
        {
            if (currentJumpState == jumpState.Crouching)
            {
                speedMultiplier = crouchingMultiplier;
            }
            else if (currentJumpState == jumpState.Sprinting)
            {
                speedMultiplier = sprintingMultiplier;
            }
        }
        float targetSpeed = moveDirection * maxMoveSpeed * speedMultiplier;
        float currentSpeed = rbody.linearVelocityX;

        // Choose acceleration/deceleration values based on whether on ground or in air
        float acceleration = onGround ? groundAcceleration : airAcceleration;
        float deceleration = onGround ? groundDeceleration : airDeceleration;

        // If trying to move or need to decelerate
        float speedDiff = targetSpeed - currentSpeed;
        float accelRate;

        if (Mathf.Abs(targetSpeed) > 0.01f)
        {
            // Accelerating
            accelRate = acceleration;
        }
        else
        {
            // Decelerating (no input)
            accelRate = deceleration;
        }

        // Apply force based on speed difference
        float movement = speedDiff * accelRate;
        rbody.AddForce(Vector2.right * movement);

        // Clamp to max speed
        float maxSpeed = maxMoveSpeed * speedMultiplier;
        if (Mathf.Abs(rbody.linearVelocityX) > maxSpeed)
        {
            rbody.linearVelocityX = Mathf.Sign(rbody.linearVelocityX) * maxSpeed;
        }
    }

    // Checks if a player can move left by checking for collisions with Solid Platform layer
    private bool CanGoLeft()
    {
        LayerMask solidPlatform = LayerMask.GetMask("Solid Platform");
        float rayDistance = 0.025f;

        float halfWidth = boxCollider.size.x * transform.localScale.x * 0.5f;
        float halfHeight = boxCollider.size.y * transform.localScale.y * 0.5f;
        Vector2 colliderCenter = (Vector2)transform.position + boxCollider.offset * transform.localScale;

        RaycastHit2D topHit = Physics2D.Raycast(new Vector2(colliderCenter.x - halfWidth, colliderCenter.y + halfHeight), Vector2.left, rayDistance, solidPlatform);
        RaycastHit2D middleHit = Physics2D.Raycast(new Vector2(colliderCenter.x - halfWidth, colliderCenter.y), Vector2.left, rayDistance, solidPlatform);
        RaycastHit2D bottomHit = Physics2D.Raycast(new Vector2(colliderCenter.x - halfWidth, colliderCenter.y - halfHeight), Vector2.left, rayDistance, solidPlatform);

        return !(topHit.collider != null || middleHit.collider != null || bottomHit.collider != null);
    }

    // Checks if a player can move right by checking for collisions with Solid Platform layer
    private bool CanGoRight()
    {
        LayerMask solidPlatform = LayerMask.GetMask("Solid Platform");
        float rayDistance = 0.025f;

        float halfWidth = boxCollider.size.x * transform.localScale.x * 0.5f;
        float halfHeight = boxCollider.size.y * transform.localScale.y * 0.5f;
        Vector2 colliderCenter = (Vector2)transform.position + boxCollider.offset * transform.localScale;

        RaycastHit2D topHit = Physics2D.Raycast(new Vector2(colliderCenter.x + halfWidth, colliderCenter.y + halfHeight), Vector2.right, rayDistance, solidPlatform);
        RaycastHit2D middleHit = Physics2D.Raycast(new Vector2(colliderCenter.x + halfWidth, colliderCenter.y), Vector2.right, rayDistance, solidPlatform);
        RaycastHit2D bottomHit = Physics2D.Raycast(new Vector2(colliderCenter.x + halfWidth, colliderCenter.y - halfHeight), Vector2.right, rayDistance, solidPlatform);

        return !(topHit.collider != null || middleHit.collider != null || bottomHit.collider != null);
    }

    // Checks if player is touching the ground
    private bool OnGround()
    {
        float halfHeight = boxCollider.size.y * transform.localScale.y * 0.5f;
        float halfWidth = boxCollider.size.x * transform.localScale.x * 0.5f;
        Vector2 colliderCenter = (Vector2)transform.position + boxCollider.offset * transform.localScale;

        float rayStartY = colliderCenter.y - halfHeight + 0.05f;
        float rayDistance = 0.2f;

        RaycastHit2D hitLeft = Physics2D.Raycast(new Vector2(colliderCenter.x - halfWidth * 0.9f, rayStartY), Vector2.down, rayDistance, groundLayers);
        RaycastHit2D hitMiddle = Physics2D.Raycast(new Vector2(colliderCenter.x, rayStartY), Vector2.down, rayDistance, groundLayers);
        RaycastHit2D hitRight = Physics2D.Raycast(new Vector2(colliderCenter.x + halfWidth * 0.9f, rayStartY), Vector2.down, rayDistance, groundLayers);

        // Debug visualization
        Debug.DrawRay(new Vector2(colliderCenter.x - halfWidth * 0.9f, rayStartY), Vector2.down * rayDistance, Color.red);
        Debug.DrawRay(new Vector2(colliderCenter.x, rayStartY), Vector2.down * rayDistance, Color.green);
        Debug.DrawRay(new Vector2(colliderCenter.x + halfWidth * 0.9f, rayStartY), Vector2.down * rayDistance, Color.blue);

        return IsValidGroundHit(hitLeft, halfHeight) || IsValidGroundHit(hitMiddle, halfHeight) || IsValidGroundHit(hitRight, halfHeight);
    }

    // Checks if a player can stand up by checking for collisions above the player
    private bool CanStandUp()
    {
        float halfWidth = boxCollider.size.x * transform.localScale.x * 0.5f;
        float checkHeight = standingCollider.y * transform.localScale.y * 0.75f;
        Vector2 colliderCenter = (Vector2)transform.position + boxCollider.offset * transform.localScale;

        RaycastHit2D hitLeft = Physics2D.Raycast(new Vector2(colliderCenter.x - halfWidth, colliderCenter.y), Vector2.up, checkHeight, groundLayers);
        RaycastHit2D hitMiddle = Physics2D.Raycast(new Vector2(colliderCenter.x, colliderCenter.y), Vector2.up, checkHeight, groundLayers);
        RaycastHit2D hitRight = Physics2D.Raycast(new Vector2(colliderCenter.x + halfWidth, colliderCenter.y), Vector2.up, checkHeight, groundLayers);

        return (hitLeft.collider == null && hitMiddle.collider == null && hitRight.collider == null);
    }

    bool IsValidGroundHit(RaycastHit2D hit, float yOffset)
    {
        if (hit.collider == null) return false;

        // Must have upward-facing normal
        if (hit.normal.y <= 0.7f) return false;

        // Hit point must be below the collider's center (not to the side)
        Vector2 colliderCenter = (Vector2)transform.position + boxCollider.offset * transform.localScale;
        if (hit.point.y >= colliderCenter.y - yOffset) return false;

        return true;
    }

    void OnMove(InputValue value)
    {
        moveDirection = value.Get<float>();
    }
    void OnJump(InputValue button)
    {
        if (button.isPressed)
        {
            if (Time.time - lastJump < 0.1)
            {
                return;
            }

            // Check if the player is pressing against walls to wall jump, otherwise just jump straight up
            if (Time.time - lastGrounded < 0.1)
            {
                // Set jump state based on current movement state before jumping
                if (isCrouching)
                {
                    currentJumpState = jumpState.Crouching;
                }
                else if (isSprinting)
                {
                    currentJumpState = jumpState.Sprinting;
                }
                else
                {
                    currentJumpState = jumpState.Walking;
                }
                _audioSouce.PlayOneShot(jumpSound);
                rbody.AddForce(Vector2.up * jumpForce);
            }
            else if (!CanGoLeft() && moveDirection < 0 && !isCrouching)
            {
                // Set jump state for wall jump
                currentJumpState = jumpState.Walking;
                rbody.linearVelocity = new Vector2(0, 0);
                rbody.AddForce(new Vector2(jumpForce, jumpForce));
                lastWallJump = Time.time;
            }
            else if (!CanGoRight() && moveDirection > 0 && !isCrouching)
            {
                // Set jump state for wall jump
                currentJumpState = jumpState.Walking;
                rbody.linearVelocity = new Vector2(0, 0);
                rbody.AddForce(new Vector2(-jumpForce, jumpForce));
                lastWallJump = Time.time;
            }
            lastJump = Time.time;
        }
        else if (rbody.linearVelocityY > 0)
        {
            rbody.linearVelocityY *= .1f;
        }
    }

    void OnSprint(InputValue button)
    {
        if (button.isPressed)
        {
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
        }
    }

    void OnCrouch(InputValue button)
    {
        if (button.isPressed)
        {
            if (!isCrouching)
            {
                StartCrouching();
            }
            else
            {
                tryStandUp = false;
            }
        }
        else
        {
            if (!isCrouching)
            {
                return;
            }
            if (CanStandUp())
            {
                StandUp();
            }
            else
            {
                tryStandUp = true;
            }
        }
    }
    void OnKnockout(InputValue button)
    {
        if (isCrouching || isSprinting)
        {
            return;
        }
        if (button.isPressed)
        {
            RaycastHit2D guardHit = Physics2D.Raycast(transform.position, facingRight ? Vector2.right : Vector2.left, 1f, guardLayer);
            if (guardHit.collider != null)
            {
                PrisonGuardStickScript guard = guardHit.collider.GetComponent<PrisonGuardStickScript>();
                if (guard != null)
                {
                    if (!guard.getHunting()) 
                        guard.KnockedOut();
                }
                GuardScript guardFlashlight = guardHit.collider.GetComponent<GuardScript>();
                if (guardFlashlight != null)
                {
                    guardFlashlight.KnockedOut();
                }
            }
        }
    }
    // Helper functions to set the player's scale and position when crouching or standing up
    private void StartCrouching()
    {
        Vector2 currentVelocity = rbody.linearVelocity;
        isCrouching = true;
        boxCollider.size = crouchingCollider;
        transform.localScale = new Vector3(0.3f, 0.3f, 1f);
        rbody.linearVelocity = currentVelocity;
    }

    private void StandUp()
    {
        Vector2 currentVelocity = rbody.linearVelocity;
        isCrouching = false;
        boxCollider.size = standingCollider;
        transform.localScale = new Vector3(0.35f, 0.35f, 1f);
        rbody.linearVelocity = currentVelocity;
    }
}