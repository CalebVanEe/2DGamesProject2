using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PrisonGuardStickScript : MonoBehaviour
{
    [Header("Targeting")]
    public LayerMask targetLayers;
    public LayerMask platformLayers;
    public float eyeDistance = 5f;
    public float huntDistance = 10f;

    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float huntSpeed = 4f;
    public float jumpPower = 12f;
    public float wallCheckDistance = 0.8f;
    public float edgeCheckDistance = 0.8f;
    public float heightThreshold = 1.0f;

    [Header("Visuals & Audio")]
    public GameObject knockedOutSprite;
    public AudioClip knockOutSound;

    private Rigidbody2D _rbody;
    private LevelSceneManagerScript _sceneManager;
    private SpriteRenderer _spriteRenderer;
    private Transform _playerTransform;

    private bool facingRight;
    private bool isHunting;
    private bool isGrounded;
    private bool wallAhead;

    private float jumpCooldown = 0f;
    private float dropCommitTimer = 0f;

    void Start()
    {
        _rbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

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

    private void HuntLogic()
    {
        if (_playerTransform == null) return;

        float xDiff = _playerTransform.position.x - transform.position.x;
        float yDiff = _playerTransform.position.y - transform.position.y;
        bool tightlyAligned = Mathf.Abs(xDiff) < 0.2f;

        // --- CASE 1: SAME HEIGHT (within threshold range) ---
        if (Mathf.Abs(yDiff) <= heightThreshold)
        {
            // Face the player
            bool playerIsRight = xDiff > 0;
            if (playerIsRight != facingRight) Flip();

            // Move towards player
            Move(huntSpeed);

            // Jump across gaps
            if (isGrounded && jumpCooldown <= 0)
            {
                Vector2 edgeOrigin = new Vector2(
                    transform.position.x + (facingRight ? edgeCheckDistance : -edgeCheckDistance),
                    transform.position.y
                );
                RaycastHit2D groundAhead = Physics2D.Raycast(edgeOrigin, Vector2.down, 1f, platformLayers);

                if (groundAhead.collider == null)
                {
                    Jump();
                }
            }
        }
        // --- CASE 2: PLAYER IS ABOVE ---
        else if (yDiff > heightThreshold)
        {
            // Face the player
            bool playerIsRight = xDiff > 0;
            if (playerIsRight != facingRight) Flip();

            // Move towards player
            Move(huntSpeed);

            if (isGrounded && jumpCooldown <= 0)
            {
                // Check for wall ahead
                RaycastHit2D wallHit = Physics2D.Raycast(
                    transform.position,
                    facingRight ? Vector2.right : Vector2.left,
                    wallCheckDistance,
                    platformLayers
                );

                // Check for gap ahead
                Vector2 edgeOrigin = new Vector2(
                    transform.position.x + (facingRight ? edgeCheckDistance : -edgeCheckDistance),
                    transform.position.y
                );
                RaycastHit2D groundAhead = Physics2D.Raycast(edgeOrigin, Vector2.down, 1f, platformLayers);

                // If wall or gap detected, jump
                if (wallHit.collider != null || groundAhead.collider == null)
                {
                    Jump();
                }
                // Special case: Guard is directly under player
                else if (tightlyAligned)
                {
                    // Raycast upward to check for platform above
                    RaycastHit2D platformAbove = Physics2D.Raycast(
                        transform.position,
                        Vector2.up,
                        yDiff + 1f, // Check up to player height + buffer
                        platformLayers
                    );

                    if (platformAbove.collider != null)
                    {
                        // Platform blocks the way - cancel hunt
                        isHunting = false;
                    }
                    else
                    {
                        // No platform blocking - jump up
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

        // Scan Right to find edge
        for (float i = 0.2f; i < maxScanDist; i += scanStep)
        {
            Vector2 checkPos = origin + (Vector2.right * i);
            RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, 1f, platformLayers);

            if (hit.collider == null)
            {
                // Found edge - calculate distance to player from this edge position
                rightEdgeDistance = Vector2.Distance(checkPos, _playerTransform.position);
                foundRightEdge = true;
                break;
            }

            // Check if wall blocks further scanning
            RaycastHit2D wallHit = Physics2D.Raycast(origin + (Vector2.right * (i - 0.1f)), Vector2.right, 0.1f, platformLayers);
            if (wallHit.collider != null) break;
        }

        // Scan Left to find edge
        for (float i = 0.2f; i < maxScanDist; i += scanStep)
        {
            Vector2 checkPos = origin + (Vector2.left * i);
            RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, 1f, platformLayers);

            if (hit.collider == null)
            {
                // Found edge - calculate distance to player from this edge position
                leftEdgeDistance = Vector2.Distance(checkPos, _playerTransform.position);
                foundLeftEdge = true;
                break;
            }

            // Check if wall blocks further scanning
            RaycastHit2D wallHit = Physics2D.Raycast(origin + (Vector2.left * (i - 0.1f)), Vector2.left, 0.1f, platformLayers);
            if (wallHit.collider != null) break;
        }

        // Return direction to nearest edge, or 0 if no edges found
        if (foundRightEdge && foundLeftEdge)
        {
            return rightEdgeDistance < leftEdgeDistance ? 1 : -1;
        }
        else if (foundRightEdge)
        {
            return 1;
        }
        else if (foundLeftEdge)
        {
            return -1;
        }

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

            RaycastHit2D wallCheck = Physics2D.BoxCast(
                boxCenter,
                boxSize,
                0f,
                checkDirection,
                0.1f,
                platformLayers
            );
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
        float horizontalBoost = (facingRight ? 1 : -1) * (isHunting ? huntSpeed : patrolSpeed);
        _rbody.linearVelocity = new Vector2(horizontalBoost, jumpPower);
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
    }

    public void KnockedOut()
    {
        if (knockedOutSprite != null)
        {
            AudioSource.PlayClipAtPoint(knockOutSound, transform.position);
            GameObject deadBody = Instantiate(knockedOutSprite, new Vector3(transform.position.x, transform.position.y - 0.45f, transform.position.z), transform.rotation);
            deadBody.GetComponent<SpriteRenderer>().flipX = _spriteRenderer.flipX;
        }
        Destroy(gameObject);
    }
}