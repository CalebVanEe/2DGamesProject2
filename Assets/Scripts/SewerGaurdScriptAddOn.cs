using UnityEngine;
using UnityEngine.SceneManagement;

public class SewerGaurdScriptAddOn : MonoBehaviour
{
    public GameObject knockOut;


    private cameraScript cameraScript;
    SewerSceneManager _sceneManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _sceneManager = FindAnyObjectByType<SewerSceneManager>();
        GameObject cameraObj = GameObject.Find("Main Camera");
        cameraScript = cameraObj.GetComponent<cameraScript>();

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && _sceneManager != null)
        {
            GameObject deadPlayer = Instantiate(knockOut, collision.transform.position, Quaternion.identity);
            deadPlayer.GetComponent<Rigidbody2D>().angularVelocity = 5f;
            cameraScript.SetPlayer(deadPlayer);
            Destroy(collision.gameObject);
            Invoke("KillPlayer", 2f);
        }

    }
    private void KillPlayer()
    {
        PlayerPrefs.SetString("KillMessage", "You got beaten to death");
        _sceneManager.playerCaught();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
