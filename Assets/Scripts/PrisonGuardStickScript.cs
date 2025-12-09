using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PrisonGuardStickScript : MonoBehaviour
{
    // Parameters
    public LayerMask targetLayers;
    public LayerMask platformLayers;
    public GameObject knockedOutSprite;
    public AudioClip knockOutSound;

    private float eyeDistance = 5f;
    private float huntDistance = 15f;
    private float patrolSpeed = 2f;
    private float huntSpeed = 4f;

    private float highJumpPower = 15f;
    private float longJumpHorizontalBoost = 11f;
    private float jumpPower;

    // NEW: Multiplier for jumping when player is above
    public float verticalJumpMultiplier = 1.5f;

    private float wallCheckDistance = 0.8f;
    private float edgeCheckDistance = 0.8f;
    private float heightThreshold = 4.0f;

    private Rigidbody2D _rbody;
    private LevelSceneManagerScript _sceneManager;
    private SpriteRenderer _spriteRenderer;
    private Transform _playerTransform;

    private bool facingRight;
    private bool isHunting;
    private bool isGrounded;
    private bool wallAhead;
    private bool isLongJumping;

    private float jumpCooldown = 0f;
    private float dropCommitTimer = 0f;

    void Start()
    {
        _rbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        jumpPower = highJumpPower;

        GameObject managerObj = GameObject.Find("LevelSceneManager");
        if (managerObj != null)
            _sceneManager = managerObj.GetComponent<LevelSceneManagerScript>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerTransform = player.transform;
        }

        facingRight = true;
    }

    void Update()
    {
        if (jumpCooldown > 0) jumpCooldown -= Time.deltaTime;
        if (dropCommitTimer > 0) dropCommitTimer -= Time.deltaTime;

        CheckGrounded();
        ScanForPlayer();
    }

    void FixedUpdate()
    {
        // Apply high horizontal speed during a long jump
        if (isLongJumping && !isGrounded)
        {
            float airSpeedMultiplier = 3.0f;
            float targetAirSpeed = huntSpeed * airSpeedMultiplier;
            float newVelocityX = (facingRight ? 1 : -1) * targetAirSpeed;

            // Only apply the boost if the current speed is less than the target speed
            float currentVelX = _rbody.linearVelocity.x;
            if (facingRight && currentVelX < newVelocityX || !facingRight && currentVelX > newVelocityX)
            {
                _rbody.linearVelocity = new Vector2(newVelocityX, _rbody.linearVelocity.y);
            }
        }
        else if (isLongJumping && isGrounded)
        {
            isLongJumping = false;
        }

        if (isHunting && _playerTransform != null)
        {
            HuntLogic();
        }
        else
        {
            PatrolLogic();
        }
    }

    private void CheckGrounded()
    {
        Vector2 groundCheckOrigin = new Vector2(transform.position.x, transform.position.y - 0.5f);
        RaycastHit2D groundHit = Physics2D.Raycast(groundCheckOrigin, Vector2.down, 0.2f, platformLayers);
        isGrounded = groundHit.collider != null;
    }

    private void ScanForPlayer()
    {
        if (_playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, _playerTransform.position);
        Vector2 direction = facingRight ? Vector2.right : Vector2.left;
        RaycastHit2D visionHit = Physics2D.Raycast(transform.position, direction, eyeDistance, targetLayers | platformLayers);

        Debug.DrawRay(transform.position, direction * eyeDistance, isHunting ? Color.red : Color.yellow);

        bool playerInSight = (visionHit.collider != null && visionHit.collider.CompareTag("Player"));

        if (distanceToPlayer <= huntDistance)
        {
            if (playerInSight)
            {
                isHunting = true;
                CaughtPlayer();
            }
        }
        else
        {
            isHunting = false;
        }
    }

    private void PatrolLogic()
    {
        Move(patrolSpeed);

        Vector2 edgeCheckOrigin = new Vector2(
            transform.position.x + (facingRight ? edgeCheckDistance : -edgeCheckDistance),
            transform.position.y
        );
        RaycastHit2D edgeHit = Physics2D.Raycast(edgeCheckOrigin, Vector2.down, 1f, platformLayers);

        if (wallAhead || edgeHit.collider == null)
        {
            Flip();
        }
    }
    public bool getHunting()
    {
        return isHunting;
    }

    private void HuntLogic()
    {
        if (_playerTransform == null) return;

        float xDiff = _playerTransform.position.x - transform.position.x;
        float yDiff = _playerTransform.position.y - transform.position.y;
        bool tightlyAligned = Mathf.Abs(xDiff) < 0.2f;

        // --- CASE 1: SAME HEIGHT (within threshold range) ---
        if (Mathf.Abs(yDiff) <= heightThreshold)
        {
            bool playerIsRight = xDiff > 0;
            if (playerIsRight != facingRight) Flip();

            Move(huntSpeed);

            if (isGrounded && jumpCooldown <= 0)
            {
                // Wall Checks
                RaycastHit2D wallMiddle = Physics2D.Raycast(transform.position, facingRight ? Vector2.right : Vector2.left, wallCheckDistance, platformLayers);
                RaycastHit2D wallBottom = Physics2D.Raycast(new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z), facingRight ? Vector2.right : Vector2.left, wallCheckDistance, platformLayers);
                RaycastHit2D wallTop = Physics2D.Raycast(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), facingRight ? Vector2.right : Vector2.left, wallCheckDistance, platformLayers);

                // Gap Check
                Vector2 edgeOrigin = new Vector2(transform.position.x + (facingRight ? edgeCheckDistance : -edgeCheckDistance), transform.position.y);
                RaycastHit2D groundAhead = Physics2D.Raycast(edgeOrigin, Vector2.down, 1f, platformLayers);

                bool isWallAhead = wallMiddle.collider != null || wallBottom.collider != null || wallTop.collider != null;
                bool isGapAhead = groundAhead.collider == null;

                if (isWallAhead)
                {
                    jumpPower = highJumpPower;
                    isLongJumping = false;
                    Jump();
                }
                else if (isGapAhead)
                {
                    jumpPower = highJumpPower / 1.5f;
                    isLongJumping = true;
                    Jump();
                }
            }
        }
        // --- CASE 2: PLAYER IS ABOVE ---
        else if (yDiff > heightThreshold)
        {
            bool playerIsRight = xDiff > 0;
            if (playerIsRight != facingRight) Flip();

            Move(huntSpeed);

            if (isGrounded && jumpCooldown <= 0)
            {
                RaycastHit2D wallHit = Physics2D.Raycast(transform.position, facingRight ? Vector2.right : Vector2.left, wallCheckDistance, platformLayers);
                Vector2 edgeOrigin = new Vector2(transform.position.x + (facingRight ? edgeCheckDistance : -edgeCheckDistance), transform.position.y);
                RaycastHit2D groundAhead = Physics2D.Raycast(edgeOrigin, Vector2.down, 1f, platformLayers);

                if (wallHit.collider != null || groundAhead.collider == null)
                {
                    // NEW: Apply multiplier when jumping up towards player
                    jumpPower = highJumpPower * verticalJumpMultiplier;
                    isLongJumping = false;
                    Jump();
                }
                else if (tightlyAligned)
                {
                    RaycastHit2D platformAbove = Physics2D.Raycast(transform.position, Vector2.up, yDiff + 1f, platformLayers);

                    if (platformAbove.collider != null)
                    {
                        isHunting = false;
                    }
                    else
                    {
                        // NEW: Apply multiplier when jumping up towards player
                        jumpPower = highJumpPower * verticalJumpMultiplier;
                        isLongJumping = false;
                        Jump();
                    }
                }
            }
        }
        // --- CASE 3: PLAYER IS BELOW ---
        else // yDiff < -heightThreshold
        {
            if (isGrounded)
            {
                int directionToEdge = FindNearestDropEdge();

                if (directionToEdge != 0)
                {
                    bool edgeIsRight = directionToEdge > 0;
                    if (edgeIsRight != facingRight) Flip();

                    Move(huntSpeed);
                    dropCommitTimer = 0.2f;
                }
                else
                {
                    bool playerIsRight = xDiff > 0;
                    if (!tightlyAligned && playerIsRight != facingRight) Flip();
                    Move(huntSpeed);
                }

                // NEW: Logic to jump over obstacles while pursuing player downwards
                if (jumpCooldown <= 0)
                {
                    RaycastHit2D wallHit = Physics2D.Raycast(
                        transform.position,
                        facingRight ? Vector2.right : Vector2.left,
                        wallCheckDistance,
                        platformLayers
                    );

                    if (wallHit.collider != null)
                    {
                        // Use standard high jump for obstacles when going down
                        jumpPower = highJumpPower;
                        isLongJumping = false;
                        Jump();
                    }
                }
            }
            else
            {
                // Air control while falling
                if (dropCommitTimer <= 0)
                {
                    bool playerIsRight = xDiff > 0;
                    float airControlSpeed = huntSpeed * 0.5f;

                    if (playerIsRight && _rbody.linearVelocity.x < airControlSpeed)
                        _rbody.linearVelocity = new Vector2(airControlSpeed, _rbody.linearVelocity.y);
                    else if (!playerIsRight && _rbody.linearVelocity.x > -airControlSpeed)
                        _rbody.linearVelocity = new Vector2(-airControlSpeed, _rbody.linearVelocity.y);
                }
            }
        }
    }

    private int FindNearestDropEdge()
    {
        if (_playerTransform == null) return 0;

        float scanStep = 0.5f;
        float maxScanDist = 8f;
        Vector2 origin = transform.position;

        float rightEdgeDistance = float.MaxValue;
        float leftEdgeDistance = float.MaxValue;
        bool foundRightEdge = false;
        bool foundLeftEdge = false;

        // Scan Right
        for (float i = 0.2f; i < maxScanDist; i += scanStep)
        {
            Vector2 checkPos = origin + (Vector2.right * i);
            RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, 1f, platformLayers);

            if (hit.collider == null)
            {
                rightEdgeDistance = Vector2.Distance(checkPos, _playerTransform.position);
                foundRightEdge = true;
                break;
            }

            RaycastHit2D wallHit = Physics2D.Raycast(origin + (Vector2.right * (i - 0.1f)), Vector2.right, 0.1f, platformLayers);
            if (wallHit.collider != null) break;
        }

        // Scan Left
        for (float i = 0.2f; i < maxScanDist; i += scanStep)
        {
            Vector2 checkPos = origin + (Vector2.left * i);
            RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, 1f, platformLayers);

            if (hit.collider == null)
            {
                leftEdgeDistance = Vector2.Distance(checkPos, _playerTransform.position);
                foundLeftEdge = true;
                break;
            }

            RaycastHit2D wallHit = Physics2D.Raycast(origin + (Vector2.left * (i - 0.1f)), Vector2.left, 0.1f, platformLayers);
            if (wallHit.collider != null) break;
        }

        if (foundRightEdge && foundLeftEdge)
        {
            return rightEdgeDistance < leftEdgeDistance ? 1 : -1;
        }
        else if (foundRightEdge) return 1;
        else if (foundLeftEdge) return -1;

        return 0;
    }

    private void Move(float speed)
    {
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        wallAhead = false;

        if (boxCollider != null)
        {
            Bounds bounds = boxCollider.bounds;
            Vector2 boxSize = new Vector2(0.1f, bounds.size.y * 0.9f);
            Vector2 checkDirection = facingRight ? Vector2.right : Vector2.left;

            Vector2 boxCenter = new Vector2(
                bounds.center.x + (facingRight ? bounds.extents.x + 0.3f : -bounds.extents.x - 0.3f),
                bounds.center.y
            );

            RaycastHit2D wallCheck = Physics2D.BoxCast(boxCenter, boxSize, 0f, checkDirection, 0.1f, platformLayers);
            if (wallCheck.collider != null)
            {
                wallAhead = true;
            }
            Vector2 targetVelocity = new Vector2((facingRight ? 1 : -1) * speed, _rbody.linearVelocity.y);
            _rbody.linearVelocity = targetVelocity;
        }
        else
        {
            Vector2 targetVelocity = new Vector2((facingRight ? 1 : -1) * speed, _rbody.linearVelocity.y);
            _rbody.linearVelocity = targetVelocity;
        }
    }

    private void Jump()
    {
        float baseSpeed = isHunting ? huntSpeed : patrolSpeed;
        float horizontalVelocity = (facingRight ? 1 : -1) * baseSpeed;

        if (isLongJumping)
        {
            horizontalVelocity += (facingRight ? 1 : -1) * longJumpHorizontalBoost;
        }

        _rbody.linearVelocity = new Vector2(horizontalVelocity, jumpPower);
        jumpCooldown = 0.5f;
    }

    private void Flip()
    {
        facingRight = !facingRight;
        _spriteRenderer.flipX = !facingRight;
    }

    private void CaughtPlayer()
    {
        // Sound or Alert effects
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && _sceneManager != null)
        {
            _sceneManager.PlayerHit();
        }
        else if (collision.gameObject.CompareTag("Crate") && !isHunting)
        {
            KnockedOut();
        }
    }

    public void KnockedOut()
    {
        if (knockOutSound != null)
        {
            AudioSource.PlayClipAtPoint(knockOutSound, transform.position);
        }

        if (knockedOutSprite != null)
        {
            GameObject deadBody = Instantiate(knockedOutSprite, new Vector3(transform.position.x, transform.position.y - 0.45f, transform.position.z), transform.rotation);
            deadBody.GetComponent<SpriteRenderer>().flipX = _spriteRenderer.flipX;
        }

        Destroy(gameObject);
    }
}