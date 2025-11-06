using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathSceneManagerScript : MonoBehaviour
{


    //public TMPro.TMP_Text Score;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        //int min = PlayerPrefs.GetInt("LevelTime") / 60;
        //int sec = PlayerPrefs.GetInt("LevelTime") % 60;
        //Score.text = min + ":" + sec;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMenuPress() {
        Debug.Log("Menu Pressed");

        SceneManager.LoadScene("MainMenu");
    }

    public void OnRetryPress() {
        //SceneManager.LoadScene("Level3-Daniel");
        string currentLevel = PlayerPrefs.GetString("CurrentLevel");    
        Debug.Log("Retrying Level: " + currentLevel);
        SceneManager.LoadScene(currentLevel);
    }
}
