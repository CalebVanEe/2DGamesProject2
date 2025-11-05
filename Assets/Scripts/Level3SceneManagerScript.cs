using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level3SceneManagerScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void playerCaught() {
        Debug.Log("Player Caught");
        SceneManager.LoadScene("Level3-Daniel");

    }

    public void playerWon() {
        Debug.Log("Player Won");
        SceneManager.LoadScene("MainMenu");

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
