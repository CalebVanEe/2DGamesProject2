using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.UI.Image;

[RequireComponent(typeof(Animator))]

public class GuardScript : MonoBehaviour
{
    public float walkingRange;
    public float flashlightLength;
    public float flashlightAngle;
    public float moveSpeed;

    Animator _animator;
    Rigidbody2D _rbody;
    SpriteRenderer _spriteRenderer;
    public GameObject caughtSprite;

    public LayerMask _playerLayer;
    public AudioClip knockOutSound;
    public GameObject knockedOutSprite;
    Level3SceneManagerScript _manager;
    private Vector2 flashLightDirection = Vector2.right;
    private float _startX;
    private bool facingRight = true;
    private float moveDirection;
    private bool caughtPlayer = false;
    private cameraScript cameraScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _manager = FindAnyObjectByType<Level3SceneManagerScript>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        cameraScript = GameObject.Find("Main Camera").GetComponent<cameraScript>();
        _startX = transform.position.x;
        moveDirection = 1f;
        _rbody = GetComponent<Rigidbody2D>();
    }



    void catchPlayer() {
        _manager.playerCaught();
    }

    // Update is called once per frame
    void Update()
    {
        lookForPlayer();
        if (transform.position.x > _startX + walkingRange && moveDirection > 0)
        {
            Flip();
        }
        else if (transform.position.x < _startX - walkingRange && moveDirection < 0) 
        {
            Flip();
        }
        else
        {
            if (!caughtPlayer)
                _rbody.linearVelocityX = moveSpeed * moveDirection;
        }

    }

    private RaycastHit2D hit;
    void lookForPlayer() {
        Vector2 hitPosition = new Vector2(transform.position.x, transform.position.y);
        hit = Physics2D.Raycast(hitPosition, flashLightDirection, flashlightLength, _playerLayer);
        if (hit.collider != null )
        {
            GameObject caughtPlayerSprite = Instantiate(caughtSprite, hit.collider.gameObject.transform.position, Quaternion.identity);
            cameraScript.SetPlayer(caughtPlayerSprite);
            Destroy(hit.collider.gameObject);
            _animator.SetBool("seesPlayer", true);
            caughtPlayer = true;
            PlayerPrefs.SetString("KillMessage", "You got caught");
            Invoke("catchPlayer", 2f);
        }
    }
    void Flip()
    {
        facingRight = !facingRight;

        if (facingRight)
        {
            flashLightDirection = Vector2.right;
        }
        else
        {
            flashLightDirection = Vector2.left;
        }

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
        moveDirection *= -1f;
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
