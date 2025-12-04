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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        wayToMove = Vector3.right * speed;
        r = GetComponent<Rigidbody2D>();
        floorLevelScript = FindAnyObjectByType<FloorLevelScript>();
    }

    // Update is called once per frame
    void Update()
    {
        //float step = speed * Time.deltaTime;
        //// calculate distance to move
        //if(Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.right, .5f, targetLayer))
        //{
        //    target = target1;
        //}
        //if (Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.left, .5f, targetLayer))
        //{
        //    target = target2;
        //}
        //transform.position = Vector3.MoveTowards(transform.position, target.transform.position, step);

       
        float step = speed * Time.deltaTime;
        if (Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.left, 4f, targetLayer))
        {
                wayToMove = Vector3.left * speed;
        }
        else if (Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.right, 4f, targetLayer))
        {
                wayToMove = Vector3.right * speed;
        
        }
        // calculate distance to move
        else if (Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.left, .5f, platformLayer))
        {
            wayToMove = Vector3.right * speed;
        }
        else if (Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.right, .5f, platformLayer))
        {
            wayToMove = Vector3.left * speed;
        }
    
        r.linearVelocity = wayToMove;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            floorLevelScript.hitMouse();
        }
    }
}
