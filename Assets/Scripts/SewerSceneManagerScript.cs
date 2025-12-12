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
    }
    public void playerCaught()
    {
        Debug.Log("Player Caught Called");
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

            PlayerPrefs.SetFloat("LevelTime", totalTime);
            PlayerPrefs.SetInt("PlayerImmune", 1); 
        door.GetComponent<Animator>().enabled = true;
            Invoke("nextScene", 1);
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
