using UnityEngine;

public class AxeScript : MonoBehaviour
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
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            PlayerPrefs.SetString("KillMessage", "You got sliced into pieces");
        levelSceneManagerScript.PlayerHit();
    }
}
