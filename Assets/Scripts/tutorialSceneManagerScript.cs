using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class tutorialSceneManagerScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    bool teachJump;
    bool teachPush;
    bool teachWallJump;
    bool teachCrouch;
    bool teachSprint;
    bool teachKnockout;
    public GameObject player;
    public TMPro.TMP_Text instructions;
    int numKeys = 1;
    int keysCollected = 0;
    public GameObject door;
    private float displayTime;
    void Start()
    {
        teachJump = false;
        teachPush = false;
        teachWallJump = false;
        teachCrouch = false;
        teachSprint = false;
        teachKnockout = false;
        door.GetComponent<Animator>().enabled = false;
        instructions.text = "Use the 'a' and 'd' keys to move";
        displayTime = Time.time;
    }
    private void Update()
    {
        if (Time.time - displayTime >= 2)
        {
            instructions.text = "";
        }
    }

    void FixedUpdate()
    {
        if (player.transform.position.x >= -6 && !teachJump)
        {
            instructions.text = "Use 'Space' to jump";
            teachJump = true;
            displayTime = Time.time;
        }
        if (player.transform.position.x >= -1 && !teachPush)
        {
            instructions.text = "Push the box by walking into it";
            teachPush = true;
            displayTime = Time.time;
        }
        if (player.transform.position.x >= 6 && !teachWallJump)
        {
            instructions.text = "Jump while pressing against walls to wall jump";
            teachWallJump = true;
            displayTime = Time.time;
        }
        if (player.transform.position.x >= 11 && !teachCrouch)
        {
            instructions.text = "Hold 's' to crouch";
            teachCrouch = true;
            displayTime = Time.time;
        }
        if (player.transform.position.x >= 24 && !teachSprint)
        {
            instructions.text = "Hold 'Shift' while moving to sprint";
            teachSprint = true;
            displayTime = Time.time;
        }
        if (player.transform.position.x >= 38 && !teachKnockout)
        {
            instructions.text = "Press 'E' to knock out guards";
            teachKnockout = true;
            displayTime = Time.time;
        }
    }

    public void Finding(GameObject g)
    {
        keysCollected++;
        Destroy(g);
        instructions.text = "Use the key to open the door and escape!";
        displayTime = Time.time;
    }
    public void hitDoor()
    {
        if (numKeys == keysCollected)
        {
            door.GetComponent<Animator>().enabled=true;
            Invoke("nextLevel", 1f);
        }
    }
    public void nextLevel()
    {
        SceneManager.LoadScene("Level1LoadingScreen");
    }
}
