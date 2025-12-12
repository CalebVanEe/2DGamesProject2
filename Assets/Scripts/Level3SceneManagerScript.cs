using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level3SceneManagerScript : MonoBehaviour
{

    private float lastLevelTime;
    private float startTime;
    public TMPro.TMP_Text timerText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lastLevelTime = PlayerPrefs.GetFloat("LevelTime");
        PlayerPrefs.SetInt("PlayerImmune", 0);
        startTime = Time.time;
    }

    public void playerCaught()
    {
        PlayerPrefs.SetFloat("LevelTime", totalTime);

        int previousLives = PlayerPrefs.GetInt("Lives");
        if (previousLives <= 1)
        {
            SceneManager.LoadScene("GameOverScene");
        }
        else {
            PlayerPrefs.SetInt("Lives", (previousLives - 1));
            SceneManager.LoadScene("DeathScene");
        }


    }

    public void playerWon() {

        levelTime = Time.time - startTime;
        totalTime = Mathf.RoundToInt(lastLevelTime + levelTime);

        PlayerPrefs.SetFloat("LevelTime", totalTime);
        SceneManager.LoadScene("GameCompletionLoadingScreen");
    }

    float levelTime;
    int totalTime;
    public void updateTimer()
    {
        levelTime = Time.time - startTime;
        totalTime = Mathf.RoundToInt(lastLevelTime + levelTime);
        int min = (totalTime / 60);
        int sec = totalTime % 60;
        if (sec < 10) {
            timerText.text = min.ToString() + ":0" + sec.ToString();
        } else { 
            timerText.text = min.ToString() + ":" + sec.ToString();
        }
    }


    // Update is called once per frame
    void Update()
    {
        updateTimer();
    }
}
