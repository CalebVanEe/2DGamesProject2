using UnityEngine;
using UnityEngine.SceneManagement;

public class WinningSceneScript : MonoBehaviour
{
    public TMPro.TMP_Text Score;
    public TMPro.TMP_Text Highscore;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        int Smin = Mathf.RoundToInt(PlayerPrefs.GetFloat("LevelTime")) / 60;
        int Ssec = Mathf.RoundToInt(PlayerPrefs.GetFloat("LevelTime")) % 60;

        int Hmin;
        int Hsec;
        Score.text = "Your Time: " + Smin + ":" + Ssec;

        if (!PlayerPrefs.HasKey("Highscore"))
        {
            PlayerPrefs.SetFloat("Highscore", PlayerPrefs.GetFloat("LevelTime"));

        } else if (PlayerPrefs.GetFloat("LevelTime") < PlayerPrefs.GetFloat("Highscore"))
        {
            PlayerPrefs.SetFloat("Highscore", PlayerPrefs.GetFloat("LevelTime"));
        }

        Hmin = Mathf.RoundToInt(PlayerPrefs.GetFloat("Highscore")) / 60;
        Hsec = Mathf.RoundToInt(PlayerPrefs.GetFloat("Highscore")) % 60;
        Highscore.text += "Highscore: " + Hmin + ":" + Hsec;
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
