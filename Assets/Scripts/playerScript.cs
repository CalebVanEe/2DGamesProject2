using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class playerScript : MonoBehaviour
{
    [Header("Movement Settings")]
    public float maxMoveSpeed = 7f;
    public float groundAcceleration = 50f;
    public float airAcceleration = 20f;
    public float groundDeceleration = 40f;
    public float airDeceleration = 10f;

    [Header("Modifiers")]
    public float sprintingMultiplier = 2f;
    public float crouchingMultiplier = 0.5f;

    [Header("Player Dimensions")]
    public float playerHeight = 1f;
    public float playerWidth = 0.5f;

    [Header("Jumping")]
    public float jumpForce = 600f;

    [Header("Scene Manager")]
    public LevelSceneManagerScript levelSceneManager;

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
    private float moveDirection = 0;
    private LayerMask groundLayers;
    private Rigidbody2D rbody;
    private BoxCollider2D boxCollider;

    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        groundLayers = LayerMask.GetMask("Platform", "Solid Platform");
        boxCollider = GetComponent<BoxCollider2D>();
        lastGrounded = 0f;
        lastJump = 0f;
        tryStandUp = false;
        isSprinting = false;
        moveDirection = 0f;
        lastWallJump = 0f;
        currentJumpState = jumpState.NoJump;
    }

    void Update()
    {

    }

    private void FixedUpdate()
    {
        // Check is player fell out of bounds
        if (transform.position.y < -10)
        {
            levelSceneManager.RespawnLevel();
        }

        // Set last grounded for jumping timings
        bool onGround = OnGround();
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
        RaycastHit2D topHit;
        RaycastHit2D middleHit;
        RaycastHit2D bottomHit;
        if (!isCrouching)
        {
            topHit = Physics2D.Raycast(new Vector2(transform.position.x - playerWidth * 0.5f, transform.position.y + playerHeight * 0.5f), Vector2.left, rayDistance, solidPlatform);
            middleHit = Physics2D.Raycast(new Vector2(transform.position.x - playerWidth * 0.5f, transform.position.y), Vector2.left, rayDistance, solidPlatform);
            bottomHit = Physics2D.Raycast(new Vector2(transform.position.x - playerWidth * 0.5f, transform.position.y - playerHeight * 0.5f), Vector2.left, rayDistance, solidPlatform);
        }
        else
        {
            topHit = Physics2D.Raycast(new Vector2(transform.position.x - playerWidth * 0.5f, transform.position.y + playerHeight * 0.25f), Vector2.left, rayDistance, solidPlatform);
            middleHit = Physics2D.Raycast(new Vector2(transform.position.x - playerWidth * 0.5f, transform.position.y), Vector2.left, rayDistance, solidPlatform);
            bottomHit = Physics2D.Raycast(new Vector2(transform.position.x - playerWidth * 0.5f, transform.position.y - playerHeight * 0.25f), Vector2.left, rayDistance, solidPlatform);
        }
        if (topHit.collider != null || middleHit.collider != null || bottomHit.collider != null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    // Checks if a player can move right by checking for collisions with Solid Platform layer
    private bool CanGoRight()
    {
        LayerMask solidPlatform = LayerMask.GetMask("Solid Platform");
        float rayDistance = 0.025f;
        RaycastHit2D topHit;
        RaycastHit2D middleHit;
        RaycastHit2D bottomHit;
        if (!isCrouching)
        {
            topHit = Physics2D.Raycast(new Vector2(transform.position.x + playerWidth * 0.5f, transform.position.y + playerHeight * 0.5f), Vector2.right, rayDistance, solidPlatform);
            middleHit = Physics2D.Raycast(new Vector2(transform.position.x + playerWidth * 0.5f, transform.position.y), Vector2.right, rayDistance, solidPlatform);
            bottomHit = Physics2D.Raycast(new Vector2(transform.position.x + playerWidth * 0.5f, transform.position.y - playerHeight * 0.5f), Vector2.right, rayDistance, solidPlatform);
        }
        else
        {
            topHit = Physics2D.Raycast(new Vector2(transform.position.x + playerWidth * 0.5f, transform.position.y + playerHeight * 0.25f), Vector2.right, rayDistance, solidPlatform);
            middleHit = Physics2D.Raycast(new Vector2(transform.position.x + playerWidth * 0.5f, transform.position.y), Vector2.right, rayDistance, solidPlatform);
            bottomHit = Physics2D.Raycast(new Vector2(transform.position.x + playerWidth * 0.5f, transform.position.y - playerHeight * 0.25f), Vector2.right, rayDistance, solidPlatform);
        }
        if (topHit.collider != null || middleHit.collider != null || bottomHit.collider != null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    // Checks if player is touching the ground
    private bool OnGround()
    {
        float yOffset = !isCrouching ? playerHeight * 0.5f : playerHeight * 0.25f;
        float rayStartY = transform.position.y - yOffset + 0.05f;
        float rayDistance = 0.2f;

        RaycastHit2D hitLeft = Physics2D.Raycast(new Vector2(transform.position.x - playerWidth * 0.45f, rayStartY), Vector2.down, rayDistance, groundLayers);
        RaycastHit2D hitMiddle = Physics2D.Raycast(new Vector2(transform.position.x, rayStartY), Vector2.down, rayDistance, groundLayers);
        RaycastHit2D hitRight = Physics2D.Raycast(new Vector2(transform.position.x + playerWidth * 0.45f, rayStartY), Vector2.down, rayDistance, groundLayers);

        return IsValidGroundHit(hitLeft, yOffset) || IsValidGroundHit(hitMiddle, yOffset) || IsValidGroundHit(hitRight, yOffset);
    }

    bool IsValidGroundHit(RaycastHit2D hit, float yOffset)
    {
        if (hit.collider == null) return false;

        // Must have upward-facing normal
        if (hit.normal.y <= 0.7f) return false;

        // Hit point must be below the player's center (not to the side)
        if (hit.point.y >= transform.position.y - yOffset) return false;

        return true;
    }

    // Checks if a player can stand up by checking for collisions above the player
    private bool CanStandUp()
    {
        RaycastHit2D hitLeft = Physics2D.Raycast(new Vector2(transform.position.x - playerWidth * 0.5f, transform.position.y), Vector2.up, playerHeight * 0.75f, groundLayers);
        RaycastHit2D hitMiddle = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.up, playerHeight * 0.75f, groundLayers);
        RaycastHit2D hitRight = Physics2D.Raycast(new Vector2(transform.position.x + playerWidth * 0.5f, transform.position.y), Vector2.up, playerHeight * 0.75f, groundLayers);
        if (hitLeft.collider == null && hitMiddle.collider == null && hitRight.collider == null)
        {
            return true;
        }
        else
        {
            return false;
        }
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

    void OnOpenDoor(InputValue button)
    {
        if (button.isPressed)
        {
            levelSceneManager.OpenAttempt(true);
        }
        else
        {
            levelSceneManager.OpenAttempt(false);
        }
    }

    // Helper functions to set the player's scale and position when crouching or standing up
    private void StartCrouching()
    {
        Vector2 currentVelocity = rbody.linearVelocity;
        isCrouching = true;
        transform.localScale = new Vector3(transform.localScale.x, playerHeight * 0.5f, transform.localScale.z);
        transform.position = new Vector3(transform.position.x, transform.position.y - playerHeight * 0.25f, transform.position.z);
        rbody.linearVelocity = currentVelocity;
    }

    private void StandUp()
    {
        Vector2 currentVelocity = rbody.linearVelocity;
        isCrouching = false;
        transform.localScale = new Vector3(transform.localScale.x, playerHeight, transform.localScale.z);
        transform.position = new Vector3(transform.position.x, transform.position.y + playerHeight * 0.25f, transform.position.z);
        rbody.linearVelocity = currentVelocity;
    }

}