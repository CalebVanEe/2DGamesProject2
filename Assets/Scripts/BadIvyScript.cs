using UnityEngine;

public class BadIvyScript : MonoBehaviour
{
    public LayerMask l;
    FloorLevelScript floorLevel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        floorLevel = FindAnyObjectByType<FloorLevelScript>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.left, 4f, l);
        RaycastHit2D hit2 = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.right, 4f, l);

       
        floorLevel.badIvy(hit, hit2, gameObject.transform.position);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        floorLevel.hitBadIvy();
    }
}
