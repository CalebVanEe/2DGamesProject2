using UnityEngine;

public class BladeScript : MonoBehaviour
{
    private LevelSceneManagerScript levelSceneManagerScript;
    private cameraScript cameraScript;
    public GameObject playerTop;
    public GameObject playerBottom;
    public float spinsPerSecond = 1f;
    private float _rotateZ;

    void Start()
    {
        _rotateZ = 0;
        levelSceneManagerScript = GameObject.Find("LevelSceneManager").GetComponent<LevelSceneManagerScript>();
        cameraScript = GameObject.Find("Main Camera").GetComponent<cameraScript>();
    }

    void Update()
    {
        _rotateZ += 360f * spinsPerSecond * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, 0, _rotateZ);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject topHalf = Instantiate(playerTop, collision.transform.position, Quaternion.identity);
            GameObject bottomHalf = Instantiate(playerBottom, collision.transform.position - Vector3.up * 0.5f, Quaternion.identity);
            topHalf.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(-2f, 1f);
            bottomHalf.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(2f, 0);
            cameraScript.SetPlayer(topHalf);
            Destroy(collision.gameObject);
            Invoke("KillPlayer", 2f);
        }
    }
    private void KillPlayer()
    {
        PlayerPrefs.SetString("KillMessage", "You got ripped apart");
        levelSceneManagerScript.PlayerHit();
    }
}