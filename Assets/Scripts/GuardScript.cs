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

    public LayerMask _playerLayer;
    private Vector2 flashLightDirection = Vector2.right;
    private float _startX;
    private bool facingRight = true;
    private float moveDirection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        _animator = GetComponent<Animator>();
        _startX = transform.position.x;
        moveDirection = 1f;
        _rbody = GetComponent<Rigidbody2D>();
    }



    void catchPlayer() {
        SceneManager.LoadScene("MainMenu");
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
            _rbody.linearVelocityX = moveSpeed * moveDirection;
        }

    }

    private RaycastHit2D hit;
    void lookForPlayer() {
        Vector2 hitPosition = new Vector2(transform.position.x, transform.position.y);



        hit = Physics2D.Raycast(hitPosition, flashLightDirection, flashlightLength, _playerLayer);

        Debug.DrawRay(hitPosition, flashLightDirection * flashlightLength, Color.red);



        if (hit.collider != null )
        {
            Debug.Log("Player Spotted");
            _animator.SetBool("seesPlayer", true);
            catchPlayer();
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


}
