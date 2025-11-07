using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour
{


    public TMPro.TMP_Text Score;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int Smin = Mathf.RoundToInt(PlayerPrefs.GetFloat("LevelTime")) / 60;
        int Ssec = Mathf.RoundToInt(PlayerPrefs.GetFloat("LevelTime")) % 60;

        int Hmin;
        int Hsec;
        if (Ssec < 10)
        {
            Score.text = "Your Time: " + Smin + ":0" + Ssec;
        }
        else
        {
            Score.text = "Your Time: " + Smin + ":" + Ssec;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnMenuPress()
    {
        Debug.Log("Menu Pressed");

        SceneManager.LoadScene("MainMenu");
    }

    public void OnCreditPress()
    {
        SceneManager.LoadScene("EndingSceneCredits");
    }
}
