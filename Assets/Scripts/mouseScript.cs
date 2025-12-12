using System;
using Unity.VisualScripting;
using UnityEngine;

public class mouseScript : MonoBehaviour
{
    float speed = .6f;
    GameObject target;
    public LayerMask targetLayer; //Player
    public LayerMask platformLayer; //MouseWall/Trap
    Vector3 wayToMove;
    Rigidbody2D r;
    FloorLevelScript floorLevelScript;
    public GameObject playerTop;
    public GameObject playerBottom;
    float lastTimeBlocked;
    bool blocked;
    bool facingRight;
    bool playerDead = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        wayToMove = Vector3.right * speed;
        facingRight = true;
        r = GetComponent<Rigidbody2D>();
        floorLevelScript = FindAnyObjectByType<FloorLevelScript>();
        lastTimeBlocked = Time.time;
        blocked = false;
        

    }

    // Update is called once per frame
    void Update()
    {
        blocked = false;
        if (Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.left, 2f, targetLayer))
        {
            //if (r.linearVelocityX > 0)
            //{
            wayToMove = Vector2.left * speed;
            blocked = true;
            //}
            //else
            //{
            //    blocked = true;
            //    lastTimeBlocked = Time.time;
            //    wayToMove = Vector2.right * speed;
            //}

        }
        else if (Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.right, 2f, targetLayer))
        {
            //if (r.linearVelocityX > 0)
            //{
            wayToMove = Vector2.right * speed;
            blocked = true;
            
        }
        // calculate distance to move
        else if (Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.left, 1f, platformLayer) && !blocked)
        {
            wayToMove = Vector3.right * speed;
            
        }
        else if (Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.right, 1f, platformLayer) && !blocked)
        {
            wayToMove = Vector3.left * speed;
            
        }
        
        r.linearVelocity = wayToMove;
        
        if (r.linearVelocityX < 0 && facingRight)
        {
            Flip();
        }
        else if (r.linearVelocityX > 0 && !facingRight)
        {
            Flip();
        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (playerDead)
                return;
            GameObject topHalf = Instantiate(playerTop, collision.transform.position, Quaternion.identity);
            GameObject bottomHalf = Instantiate(playerBottom, collision.transform.position - Vector3.up * 0.5f, Quaternion.identity);
            topHalf.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(-2f, 1f);
            bottomHalf.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(2f, 0);
            GameObject player = collision.gameObject;
            player.GetComponent<SpriteRenderer>().enabled = false;
            playerDead = true;
            Invoke("KillPlayer", 2f);
        }
    }
    private void KillPlayer()
    {
        floorLevelScript.hitMouse();
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
