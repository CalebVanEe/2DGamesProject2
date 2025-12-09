using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelSceneManagerScript : MonoBehaviour
{
    public Vector3 Door1Location = new Vector3(153f, 13, 0);
    public playerScript player;
    public GameObject door;
    public TMP_Text timerText;
    private bool levelKeyCollected;
    private int totalTime;
    private float lastLevelTime;
    private float startTime;
    private bool openingDoor;
    public int level;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        openingDoor = false;
        levelKeyCollected = false;
        door.GetComponent<Animator>().enabled = false;
        lastLevelTime = PlayerPrefs.GetFloat("LevelTime");
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        updateTimer();
    }
    public void updateTimer()
    {
        float levelTime = Time.time - startTime;
        totalTime = Mathf.RoundToInt(lastLevelTime + levelTime);
        int min = (totalTime / 60);
        int sec = totalTime % 60;
        if (sec < 10)
        {
            timerText.text = min.ToString() + ":0" + sec.ToString();
        }
        else
        {
            timerText.text = min.ToString() + ":" + sec.ToString();
        }
    }
    public void PlayerHit()
    {
        PlayerPrefs.SetFloat("LevelTime", totalTime);

        int previousLives = PlayerPrefs.GetInt("Lives");
        if (previousLives <= 1)
        {
            SceneManager.LoadScene("GameOverScene");
        }
        else
        {

            PlayerPrefs.SetInt("Lives", (previousLives - 1));
            SceneManager.LoadScene("DeathScene");
        }
    }
    public void OpenAttempt(bool open)
    {
        openingDoor = open;
    }
    public bool GetOpenAttempt()
    {
        return openingDoor;
    }
    public void LevelCompleted()
    {
        if (!levelKeyCollected)
        {
            return;
        }
        else
        {
            door.GetComponent<Animator>().enabled = true;
            Invoke("NextScene", 2);
        }
    }
    private void NextScene()
    {
        PlayerPrefs.SetFloat("LevelTime", totalTime);
        if (level == 1)
            SceneManager.LoadScene("Level2LoadingScreen");
        if (level == 3)
            SceneManager.LoadScene("Level4LoadingScreen");
    }
    public void KeyCollected()
    {
        levelKeyCollected = true;
    }
}