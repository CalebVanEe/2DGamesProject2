using UnityEngine;
using UnityEngine.SceneManagement;

public class SewerSceneManager : MonoBehaviour
{
    public GameObject bulletPrefab;
    ObjectPool bulletpool;
    public GameObject door;


    cameraPanningScript cameraPanningScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private float lastLevelTime;
    private float startTime;
    public TMPro.TMP_Text timerText;

    void Start()
    {

        lastLevelTime = PlayerPrefs.GetFloat("LevelTime");
        startTime = Time.time;
        PlayerPrefs.SetInt("PlayerImmune", 0);

        bulletpool = new ObjectPool(bulletPrefab, true, 20);


    }

    float levelTime;
    int totalTime;
    public void updateTimer()
    {
        levelTime = Time.time - startTime;
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
    public void playerCaught()
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
    // Update is called once per frame
    void Update()
    {
        updateTimer();
    }

    public void hitDoor()
    {

            door.GetComponent<Animator>().enabled = true;
            Invoke("nextScene", 2);
    }

    public void nextScene()
    {
        PlayerPrefs.SetFloat("LevelTime", totalTime);
        SceneManager.LoadScene("Level6LoadingScreen");

    }
    public GameObject GetBullet()
    {
        return bulletpool.GetObject();
    }
}
