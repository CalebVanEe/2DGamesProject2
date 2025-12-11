using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Threading;
using TMPro;

public class SceneManagerscript : MonoBehaviour
{
    public GameObject bookshelf;
    public TMP_Text timerText;
    Vector3 temp;
    int keysExist = 2;
    int keysFound = 0;
    public GameObject key1;
    public GameObject key2;
    public GameObject door;
    public GameObject bat;
    float batTime;
    public GameObject player;
    float speed = 4f;
    List<GameObject> bats = new List<GameObject>();
    bool gotToPlayer;
    private int totalTime;
    private float lastLevelTime;
    private float startTime;
    float secondsSince = 0;
    float display = 0;
    bool doorDone = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gotToPlayer = false;
        temp = bookshelf.transform.position;
        key1.GetComponent<Collider2D>().enabled = false;
        key2.GetComponent<Collider2D>().enabled = false;
        door.GetComponent<Animator>().enabled = false;
        batTime = Time.time;
        startTime = Time.time; 
        lastLevelTime = PlayerPrefs.GetFloat("LevelTime");
    }

    // Update is called once per frame
    void Update()
    {
        updateTimer();
        if (Time.time - batTime > 6)
        {
            batTime = Time.time;
            spawnBat();
        }
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
    public void foundKey()
    {
        keysFound++;
    }

    public void trap(RaycastHit2D hit, RaycastHit2D hit2, RaycastHit2D hit3)
    {
        if (hit.collider != null || hit2.collider != null || hit3.collider != null)
        {
            if (hit.collider != null && hit.collider.gameObject.CompareTag("Crate"))
            {
                bookshelf.transform.position = new Vector3(temp.x - 3, temp.y, temp.z);
                if (key1 != null)
                {
                    key1.GetComponent<Collider2D>().enabled = true;
                }
            }
            if (hit2.collider != null && hit2.collider.gameObject.CompareTag("Crate"))
            {
                bookshelf.transform.position = new Vector3(temp.x - 3, temp.y, temp.z);
                if (key1 != null)
                {
                    key1.GetComponent<Collider2D>().enabled = true;
                }
            }
            if (hit3.collider != null && hit3.collider.gameObject.CompareTag("Crate"))
            { 
                
                bookshelf.transform.position = new Vector3(temp.x - 3, temp.y, temp.z);
                if (key1 != null)
                {
                    key1.GetComponent<Collider2D>().enabled = true;
                }
            }

        }
        else
        {
            bookshelf.transform.position = temp;
            if (key1 != null)
            {
                key1.GetComponent<Collider2D>().enabled = false;
            }
        }
    }


    public void changeKey()
    {
        key2.GetComponent<Collider2D>().enabled = true;
    }
    public void key2Found(GameObject g)
    {
        Destroy(g);
        foundKey();
    }
    public void hitDoor() { 
        if(keysFound == keysExist)
        {
            doorDone = true;
            door.GetComponent<Animator>().enabled = true;
            Invoke("nextScene", 2);

        }
    }

    public void spawnBat()
    {
        Debug.Log("spawned a bat");
        float x = UnityEngine.Random.Range(player.transform.position.x - 5, player.transform.position.x + 5);
        
        GameObject b = Instantiate(bat, new Vector2(x, 8), Quaternion.identity);
        



    }
    public void help(GameObject g)
    {
        gotToPlayer = true;
    }
    public bool getGotToPlayer()
    {
        return gotToPlayer;
    }
    public Vector3 getPlayerPosition()
    {
        return player.transform.position;
    }
    public void destroyBat(GameObject bat)
    {
        Destroy(bat);
    }
    public void nextScene()
    {
        PlayerPrefs.SetFloat("LevelTime", totalTime);
        SceneManager.LoadScene("Level5LoadingScreen");
        
    }
    
    public void batHitPlayer()
    {
        if (!doorDone)
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
                PlayerPrefs.SetString("KillMessage", "You got bit by a bat");
                SceneManager.LoadScene("DeathScene");
            }
        }
    }
}
