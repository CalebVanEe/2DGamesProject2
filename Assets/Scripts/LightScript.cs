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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _manager = FindAnyObjectByType<Level3SceneManagerScript>();
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



        if ( hit1.collider != null && hit1.collider.CompareTag("Player") || hit2.collider != null && hit2.collider.CompareTag("Player"))
        {
            _manager.playerCaught();

        }
    }
}


