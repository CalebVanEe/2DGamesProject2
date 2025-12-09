using System;
using Unity.VisualScripting;
using UnityEngine;

public class mouseScript : MonoBehaviour
{
    float speed = .8f;
    public GameObject player;
    GameObject target;
    public LayerMask targetLayer;
    public LayerMask platformLayer;
    Vector3 wayToMove;
    Rigidbody2D r;
    FloorLevelScript floorLevelScript;
    float lastTimeBlocked;
    bool blocked;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        wayToMove = Vector3.right * speed;
        r = GetComponent<Rigidbody2D>();
        floorLevelScript = FindAnyObjectByType<FloorLevelScript>();
        lastTimeBlocked = Time.time;
        blocked = false;

    }

    // Update is called once per frame
    void Update()
    {
        if(blocked && (Time.time - lastTimeBlocked > 2f))
        {
            blocked = false;
        }

        if (Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.left, 4f, targetLayer))
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

            Debug.Log("see a man");
        }
        else if (Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.right, 4f, targetLayer))
        {
            //if (r.linearVelocityX > 0)
            //{
            wayToMove = Vector2.right * speed;
            blocked = true;
            //}
            //else
            //{
            //    blocked = true;
            //    lastTimeBlocked = Time.time;
            //    wayToMove = Vector2.left * speed;
            //}
            Debug.Log("see a man");
        }
        // calculate distance to move
        else if (Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.left, .5f, platformLayer) && !blocked)
        {
            wayToMove = Vector3.right * speed;
            Debug.Log("see a wall");
            blocked = true;
        }
        else if (Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.right, .5f, platformLayer) && !blocked)
        {
            Debug.Log("see a wall");
            wayToMove = Vector3.left * speed;
        }
        
        r.linearVelocity = wayToMove;
        blocked = false;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            floorLevelScript.hitMouse();
        }
    }
}
