using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class tutorialSceneManagerScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    bool teachCrouch;
    bool teachKey;
    public GameObject player;
    public TMPro.TMP_Text instructions;
    int numKeys = 1;
    int keysCollected = 0;
    public GameObject door;
    bool teachSprint;
    void Start()
    {
        teachCrouch = false;
        teachSprint = false;
        door.GetComponent<Animator>().enabled = false;
        teachKey = false;
        instructions.text = "Use the 'a' and 'd' keys to move";
        Invoke("dissapearText", 2);
        instructions.text = "Move the crates to create stairs in case you need them later";
        instructions.enabled = true;
        Invoke("dissapearText", 2);
    }

    // Update is called once per frame
    void Update()
    {
        if ((player.transform.position.x >= -3 && player.transform.position.x <= -1.5) && !teachCrouch)
        {
            instructions.text = "Use the 's' key to crouch and inspect what's under the wall";
            instructions.enabled = true;
            teachCrouch = true;
            Invoke("dissapearText", 2);
        }

        if(player.transform.position.x >=4 && player.transform.position.x <=5 && player.transform.position.y <= 2 && player.transform.position.y > 0 && !teachSprint)
        {
            instructions.text = "Use the 'shift' key to sprint";
            instructions.enabled = true;
            teachSprint = true;
            Invoke("dissapearText", 2);
        }
        

    }

    public void dissapearText()
    {
        instructions.enabled = false;
    }

    public void Finding(GameObject g)
    {
        keysCollected++;
        Destroy(g);
        instructions.text = "Use the key to open the door and escape!";
        instructions.enabled = true;
        Invoke("dissapearText", 2);
        
    }
    public void hitDoor()
    {
        if (numKeys == keysCollected)
        {
            door.GetComponent<Animator>().enabled=true;
            Invoke("nextLevel", 1);
        }
    }
    public void nextLevel()
    {
        SceneManager.LoadScene("Level1LoadingScreen");
    }
}
