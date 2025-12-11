using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class LightScript : MonoBehaviour
{
    public float speed;
    public float angleRange;
    public GameObject _light;
    public float innerSpotAngle;
    public float lightDistance;
    Level3SceneManagerScript _manager;
    public GameObject caughtSprite;
    cameraScript cameraScript;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _manager = FindAnyObjectByType<Level3SceneManagerScript>();
        cameraScript = GameObject.Find("Main Camera").GetComponent<cameraScript>();
    }

    private float _time;

    void Update()
    {
        panLight();
        catchPlayer();
    }

    void panLight()
    {

        _time += Time.deltaTime * speed;

        float angle = Mathf.Sin(_time) * angleRange;

        _light.transform.rotation = Quaternion.Euler(0f, 0f, angle);


    }

    void catchPlayer()
    {
        Vector2 hit1Direction = Quaternion.Euler(0, 0, -innerSpotAngle) * -transform.up;
        Vector2 hit2Direction = Quaternion.Euler(0, 0, innerSpotAngle) * -transform.up;


        RaycastHit2D hit1 = Physics2D.Raycast(transform.position, hit1Direction, lightDistance);
        RaycastHit2D hit2 = Physics2D.Raycast(transform.position, hit2Direction, lightDistance);



        if (hit1.collider != null && hit1.collider.CompareTag("Player") || hit2.collider != null && hit2.collider.CompareTag("Player"))
        {
            GameObject caughtPlayerSprite = null;
            if (hit1.collider != null)
            {
                caughtPlayerSprite = Instantiate(caughtSprite, hit1.collider.gameObject.transform.position, Quaternion.identity);
                Destroy(hit1.collider.gameObject);
            }
            else
            {
                caughtPlayerSprite = Instantiate(caughtSprite, hit2.collider.gameObject.transform.position, Quaternion.identity);
                Destroy(hit2.collider.gameObject);
            }
            cameraScript.SetPlayer(caughtPlayerSprite);
            PlayerPrefs.SetString("KillMessage", "You got caught");
            Invoke("caughtPlayer", 2f);
        }
    }
    void caughtPlayer()
    {
        _manager.playerCaught();
    }
    
}


