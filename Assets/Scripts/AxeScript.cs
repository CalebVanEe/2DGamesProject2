using UnityEngine;
using UnityEngine.LowLevel;

public class AxeScript : MonoBehaviour
{
    private LevelSceneManagerScript levelSceneManagerScript;
    private cameraScript cameraScript;
    public GameObject playerTop;
    public GameObject playerBottom;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        levelSceneManagerScript = GameObject.Find("LevelSceneManager").GetComponent<LevelSceneManagerScript>();
        cameraScript = GameObject.Find("Main Camera").GetComponent<cameraScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject topHalf = Instantiate(playerTop, collision.transform.position, Quaternion.identity);
            GameObject bottomHalf = Instantiate(playerBottom, collision.transform.position - Vector3.up, Quaternion.identity);
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
