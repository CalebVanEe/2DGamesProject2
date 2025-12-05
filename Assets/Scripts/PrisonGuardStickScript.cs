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
    public float wallCheckDistance = 0.8f; // Now only used as a reference/ignored in PatrolLogic
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
    private bool wallAhead; // <-- NEW: Flag to track if the Move function hit a wall

    // --- NEW VARIABLES ---
    private float jumpCooldown = 0f;
    private float dropCommitTimer = 0f; // Forces guard to commit to a fall

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
        if (dropCommitTimer > 0) dropCommitTimer -= Time.deltaTime; // Count down the commit timer

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
        Move(patrolSpeed); // This call sets the 'wallAhead' flag

        // Original wall check is removed here, relying on the flag set by Move()

        Vector2 edgeCheckOrigin = new Vector2(
            transform.position.x + (facingRight ? edgeCheckDistance : -edgeCheckDistance),
            transform.position.y
        );
        RaycastHit2D edgeHit = Physics2D.Raycast(edgeCheckOrigin, Vector2.down, 1f, platformLayers);

        // Check if wallAhead is TRUE (set by Move()), OR if there is an edge
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

        // Check for an obstacle directly above the guard
        RaycastHit2D ceilingObstacle = Physics2D.Raycast(transform.position, Vector2.up, 2.5f, platformLayers);
        bool jumpIsBlocked = ceilingObstacle.collider != null;

        // --- STATE 1: Player is ABOVE or SAME HEIGHT ---
        if (yDiff > -heightThreshold)
        {
            // **PRIMARY FIX FOR FLIPPING/STUCKING:**
            // If we are tightly aligned, the player is significantly above us (implying a platform separation),
            // AND a jump is blocked or we're just not high enough to warrant a jump yet,
            // we must break the vertical alignment by changing direction.

            bool playerIsRight = xDiff > 0;

            // Only flip to follow the player's X position if we are NOT tightly aligned.
            // The check above handles the tightlyAligned case when we are stuck.
            if (!tightlyAligned && playerIsRight != facingRight) Flip();

            Move(huntSpeed);

            if (isGrounded && jumpCooldown <= 0)
            {
                RaycastHit2D wallHit = Physics2D.Raycast(transform.position, facingRight ? Vector2.right : Vector2.left, wallCheckDistance, platformLayers);
                Vector2 edgeOrigin = new Vector2(transform.position.x + (facingRight ? edgeCheckDistance : -edgeCheckDistance), transform.position.y);
                RaycastHit2D groundAhead = Physics2D.Raycast(edgeOrigin, Vector2.down, 1f, platformLayers);

                // Condition 1: Jump to bypass a wall or a gap (normal pathfinding)
                if (wallHit.collider != null || groundAhead.collider == null)
                {
                    // If we are stuck right under the player with a block overhead, 
                    // the logic above should have flipped us already. Now we just jump normally 
                    // to progress if the path is blocked by a wall or edge (not the player being directly above).
                    Jump();
                }
                // Condition 2: Jump when tightly aligned beneath player, but only if the jump isn't blocked.
                else if (yDiff > heightThreshold * 1.5f && tightlyAligned)
                {
                    if (!jumpIsBlocked)
                    {
                        Jump();
                    }
                }
            }
        }
        // --- STATE 2: Player is BELOW ---
        else
        {
            // ... State 2 code remains unchanged ...
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
                // --- AIR CONTROL ---
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
        float scanStep = 0.5f;
        float maxScanDist = 8f;
        Vector2 origin = transform.position;

        // Scan Right
        // OPTIMIZATION: Start scanning closer (0.2f) so we detect the edge when we are standing right on it
        for (float i = 0.2f; i < maxScanDist; i += scanStep)
        {
            Vector2 checkPos = origin + (Vector2.right * i);
            RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, 1f, platformLayers);
            if (hit.collider == null) return 1;

            RaycastHit2D wallHit = Physics2D.Raycast(origin + (Vector2.right * (i - 0.1f)), Vector2.right, 0.1f, platformLayers);
            if (wallHit.collider != null) break;
        }

        // Scan Left
        for (float i = 0.2f; i < maxScanDist; i += scanStep)
        {
            Vector2 checkPos = origin + (Vector2.left * i);
            RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, 1f, platformLayers);
            if (hit.collider == null) return -1;

            RaycastHit2D wallHit = Physics2D.Raycast(origin + (Vector2.left * (i - 0.1f)), Vector2.left, 0.1f, platformLayers);
            if (wallHit.collider != null) break;
        }

        return 0;
    }

    private void Move(float speed)
    {
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        wallAhead = false; // <-- RESET FLAG HERE

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
        // Fallback block remains unchanged for safety
        else
        {
            Vector2 targetVelocity = new Vector2((facingRight ? 1 : -1) * speed, _rbody.linearVelocity.y);
            _rbody.linearVelocity = targetVelocity;
        }
    }

    private void Jump()
    {
        _rbody.linearVelocity = new Vector2(_rbody.linearVelocity.x, jumpPower);
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
            Instantiate(knockedOutSprite, new Vector3(transform.position.x, transform.position.y - 0.45f, transform.position.z), transform.rotation);
        }
        Destroy(gameObject);
    }
}