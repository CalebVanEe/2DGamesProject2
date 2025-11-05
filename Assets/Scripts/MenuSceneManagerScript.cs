using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSceneManagerScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void hitTutorial()
    {
        SceneManager.LoadScene("TutorialScene");
    }
    public void hitPlay()
    {
        SceneManager.LoadScene("Level1-Caleb");
    }
}
