using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuSceneManagerScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerPrefs.SetInt("PlayerImmune", 0);
        PlayerPrefs.SetInt("Lives", 7);
        PlayerPrefs.SetFloat("LevelTime", 0);
        PlayerPrefs.SetString("CurrentLevel", "Level1-Caleb");
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
        Application.Quit();
        }
    }

    public void hitTutorial()
    {
        SceneManager.LoadScene("TutorialScene");
    }
    public void hitPlay()
    {
        SceneManager.LoadScene("Level1LoadingScreen");
    }
}
