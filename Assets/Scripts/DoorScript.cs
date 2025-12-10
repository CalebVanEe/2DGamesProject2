using UnityEngine;

public class DoorScript : MonoBehaviour
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
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            levelSceneManagerScript.LevelCompleted();
        }
    }
}