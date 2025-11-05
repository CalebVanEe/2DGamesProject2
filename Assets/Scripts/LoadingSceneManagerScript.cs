using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadingSceneManagerScript : MonoBehaviour
{
    public string Level1Message = "Welcome to Level 1!";
    public string Level2Message = "You've escaped the Torture Chamber, and find yourself locked in the Warden's Office";
    public int levelIndex;
    public TMP_Text messageText;
    private string currentMessage;

    void Start()
    {
        DisplayMessage();
    }

    void DisplayMessage()
    {
        if (levelIndex == 1)
        {
            currentMessage = Level1Message;
        }
        else if (levelIndex == 2)
        {
            currentMessage = Level2Message;
        }

        // Start the coroutine to display text character by character
        StartCoroutine(TypeText(currentMessage));
    }

    IEnumerator TypeText(string message)
    {
        messageText.text = ""; // Clear the text first

        foreach (char letter in message)
        {
            messageText.text += letter;
            yield return new WaitForSeconds(0.05f); // 50ms = 0.05 seconds
        }
    }
}