using UnityEngine;
using UnityEngine.Rendering;

public class BadIvyScript : MonoBehaviour
{
    public LayerMask l;
    FloorLevelScript floorLevel;
    Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        floorLevel = FindAnyObjectByType<FloorLevelScript>();
        animator = GetComponent<Animator>();
        animator.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.left, 3f, l);
        RaycastHit2D hit2 = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.right, 3f, l);

        if(hit || hit2)
        {
            animator.enabled = true;
        }
        floorLevel.badIvy(hit, hit2, gameObject.transform.position);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        floorLevel.hitBadIvy();
    }
}
