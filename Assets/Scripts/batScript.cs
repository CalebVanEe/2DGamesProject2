using UnityEngine;

public class batScript : MonoBehaviour
{
    float speed = 6f;
    //public GameObject player;
    Vector3 p;
    SceneManagerscript scene;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       

        scene = FindAnyObjectByType<SceneManagerscript>();
        p = scene.getPlayerPosition();
    }

    // Update is called once per frame
    void Update()
    {
        float m = speed * Time.deltaTime;
        if (!scene.getGotToPlayer())
        {
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, p, m);

            if (gameObject.transform.position == p)
            {
                scene.help(gameObject);
            }
        }
        if (scene.getGotToPlayer())
        {
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, new Vector3(gameObject.transform.position.x - 1, -4, 0), m);

        }
        if (gameObject.transform.position.y == 0)
        {
            scene.destroyBat(gameObject);
        }


    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            scene.batHitPlayer();
        }
        else if (!collision.gameObject.CompareTag("Painting"))
        {
            Destroy(gameObject);
        }

    }
   
}
