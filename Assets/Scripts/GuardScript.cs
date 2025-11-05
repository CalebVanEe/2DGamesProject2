using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]

public class GuardScript : MonoBehaviour
{

    public float walkingRange;
    public float flashlightLengh;
    public float flashlightAngle;
    public float moveSpeed;

    Animator _animator;

    Rigidbody2D _rbody;
    private float _startX;
    private bool facingRight = true;
    private float moveDirection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Flip();

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
        //if (transform.position.x > _startX + walkingRange)
        //{
        //    Flip();
        //}
        //else if (transform.position.x < _startX - walkingRange)
        //{
        //    Flip();
        //}
        //else {
        //    _rbody.linearVelocityX = moveSpeed * moveDirection;
        //}
        _rbody.linearVelocityX = moveSpeed * -moveDirection;

    }

    void lookForPlayer() {
        Vector2 hit1Direction = Quaternion.Euler(0, 0, -flashlightAngle) * -transform.up;
        Vector2 hit2Direction = Quaternion.Euler(0, 0, flashlightAngle) * -transform.up;


        RaycastHit2D hit1 = Physics2D.Raycast(transform.position, hit1Direction, flashlightLengh);
        RaycastHit2D hit2 = Physics2D.Raycast(transform.position, hit2Direction, flashlightLengh);



        if (hit1.collider != null && hit1.collider.CompareTag("Player") || hit2.collider != null && hit2.collider.CompareTag("Player"))
        {
            _animator.SetBool("seesPlayer", true);
            Invoke("catchPlayer", 1f);
        }
    }
    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
        moveDirection *= -1f;
    }


}
