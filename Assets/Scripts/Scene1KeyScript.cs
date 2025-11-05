using UnityEngine;

public class Scene1KeyScript : MonoBehaviour
{
    private LevelSceneManagerScript levelSceneManagerScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        levelSceneManagerScript = GameObject.Find("LevelSceneManager").GetComponent<LevelSceneManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            levelSceneManagerScript.KeyCollected();
            Destroy(gameObject);
        }
    }
}