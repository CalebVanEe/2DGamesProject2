using TMPro;
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
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      
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
        if(check == 4)
        {
            codeInput.enabled = true;
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
    public void badIvy(RaycastHit2D hit, RaycastHit2D hit2, Vector2 p)
    {
        if(hit|| hit2)
        {
           
            Debug.Log("getting closer");
         
            warning.transform.position = p;
            warning.Play();
        }
    }

    public void hitBadIvy()
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
    public void hitGoodIvy(GameObject g)
    {
        if (check == 0) {
            lyk.text = "The first letter of the code is: " + code[check].ToString();
        }
        else
        {
            lyk.text = "The next letter of the code is: " + code[check].ToString();
        }
        lyk.enabled = true;
        check++;
        g.GetComponent<Collider2D>().enabled = false;
    }

    public void hitDoor()
    {
        codeInput.enabled = true;
        Debug.Log("Ouch");

    }

    public void endText()
    {
        Debug.Log(code);
        if (codeInput.text.Equals(code))
        {
            Debug.Log("The correct Code");
            door.GetComponent<Collider2D>().enabled = false;
            door.GetComponent<Animator>().enabled = true;


            //SceneManager.LoadScene("Level3-Daniel");
        }
    }
}

