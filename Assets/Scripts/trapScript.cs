using Unity.Burst.CompilerServices;
using UnityEngine;

public class trapScript : MonoBehaviour
{
    SceneManagerscript scene;
    public LayerMask trapLayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scene = FindAnyObjectByType<SceneManagerscript>();   
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x + 1.5f, transform.position.y), Vector2.up, 1f, trapLayer);
        RaycastHit2D hit2 = Physics2D.Raycast(new Vector2(transform.position.x - 1.5f, transform.position.y), Vector2.up, 1f, trapLayer);
        RaycastHit2D hit3 = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.up, 1f, trapLayer);

        scene.trap(hit, hit2, hit3);

    }

 
}
