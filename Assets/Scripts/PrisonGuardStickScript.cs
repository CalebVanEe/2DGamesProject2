using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PrisonGuardStickScript : MonoBehaviour
{
    public LayerMask targetLayers;
    public LayerMask platformLayers;
    public GameObject knockedOutSprite;
    public AudioClip knockOutSound;
    public float patrolSpeed = 2f;
    public float wallCheckDistance = 0.6f;
    public float edgeCheckDistance = 0.6f;
    public float eyeDistance = 5f;

    private Rigidbody2D _rbody;
    private LevelSceneManagerScript _sceneManager;
    private SpriteRenderer _spriteRenderer;
    private bool facingRight;

    void Start()
    {
        _rbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _sceneManager = GameObject.Find("LevelSceneManager").GetComponent<LevelSceneManagerScript>();
        facingRight = true;
    }

    void Update()
    {
        // Raycast to check for player at close range (for immediate collision detection)
        RaycastHit2D closePlayerHit = Physics2D.Raycast(transform.position, facingRight ? Vector2.right : Vector2.left, wallCheckDistance, targetLayers);
        if (closePlayerHit.collider != null && closePlayerHit.collider.CompareTag("Player"))
        {
            CaughtPlayer();
            return;
        }

        // Raycast to check for player at eye distance (vision range)
        RaycastHit2D visionHit = Physics2D.Raycast(transform.position, facingRight ? Vector2.right : Vector2.left, eyeDistance, targetLayers | platformLayers);

        // Debug visualization for vision raycast
        Debug.DrawRay(transform.position, (facingRight ? Vector2.right : Vector2.left) * eyeDistance, Color.yellow);

        if (visionHit.collider != null && visionHit.collider.CompareTag("Player"))
        {
            CaughtPlayer();
        }
    }

    private void FixedUpdate()
    {
        // Check for wall ahead
        Vector2 wallCheckOrigin = transform.position;
        RaycastHit2D wallHit = Physics2D.Raycast(wallCheckOrigin, facingRight ? Vector2.right : Vector2.left, wallCheckDistance, platformLayers);

        // Check for edge ahead
        Vector2 edgeCheckOrigin = new Vector2(
            transform.position.x + (facingRight ? edgeCheckDistance : -edgeCheckDistance),
            transform.position.y
        );
        RaycastHit2D edgeHit = Physics2D.Raycast(edgeCheckOrigin, Vector2.down, 1f, platformLayers);

        // If hit a wall or no ground ahead, turn around
        if (wallHit.collider != null || edgeHit.collider == null)
        {
            facingRight = !facingRight;
            _spriteRenderer.flipX = !facingRight;
        }

        // Move in current direction
        _rbody.linearVelocityX = (facingRight ? 1 : -1) * patrolSpeed;
    }

    private void CaughtPlayer()
    {
        // To be implemented
        Debug.Log("Player caught!");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            CaughtPlayer();
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