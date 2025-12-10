using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class FloorLevelScript : MonoBehaviour
{
    public ParticleSystem warning;
    private int totalTime;
    private float lastLevelTime;
    private float startTime;
    public TMP_Text timerText;
    string code = "";
    int check = 0;
    public TMP_Text lyk;
    public TMP_InputField codeInput;
    public GameObject door;
    public InputField cod;
    bool doorGotHit = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        door.GetComponent<Collider2D>().enabled = true;
        door.GetComponent<Animator>().enabled = false;
        //cod.enabled = false;
        lyk.enabled = false;
   
        startTime = Time.time;
        lastLevelTime = PlayerPrefs.GetFloat("LevelTime");
        for (int i = 0; i < 4; i++)
        {
            int num = UnityEngine.Random.Range(0, 10);
            code += num.ToString();
        }
        
    }
    

    // Update is called once per frame
    void Update()
    {
        if (codeInput.text.Equals(code))
        {
            lyk.text = "That is the correct code! Exit through the door!";
            lyk.enabled = true;
            Invoke("removeText", 2);
        }
    }

    public void OnClick()
    {
        SceneManager.LoadScene("PauseScene");
        PlayerPrefs.SetFloat("LevelTime", totalTime);
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
    public void badIvy(Vector2 p)
    {
       
            warning.transform.position = p;
            warning.Play();
       
    }

    public void hitMouse()
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
            PlayerPrefs.SetString("KillMessage", "You got bit by a mouse");
            SceneManager.LoadScene("DeathScene");
        }
    }
    public void hitGoodIvy(GameObject g)
    {
        if (check == 0) {
            lyk.text = "The first number of the code is: " + code[check].ToString();
        }
        else
        {
            lyk.text = "The fragments of code found: " + code.Substring(0, check + 1).ToString();
        }
        lyk.enabled = true;
        check++;
        g.GetComponent<Collider2D>().enabled = false;
        Invoke("removeText", 2);
    }
    private void removeText()
    {
        lyk.enabled = false;
    }

    public void hitDoor()
    { 
        if (codeInput.text.Equals(code))
        {
            door.GetComponent<Collider2D>().enabled = false;
            door.GetComponent<Animator>().enabled = true;


            SceneManager.LoadScene("Level4LoadingScene");
        }
        else
        {
            lyk.text = "Enter the correct code";
            lyk.enabled = true;
            Invoke("removeText", 2);
        }
    }
}

