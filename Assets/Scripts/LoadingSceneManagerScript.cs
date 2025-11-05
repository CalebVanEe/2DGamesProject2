using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadingSceneManagerScript : MonoBehaviour
{
    public string Level1Message = "You are in the depths of the prison in the torture chamber. You must find your way out to survive";
    public string Level2Message = "You've escaped the Torture Chamber, and find yourself locked in the Warden's Office";
    public string Level3Message = "You've made your way out to the prison yard. The guards have discovered your absence and are looking for you. Escape or die";
    public string CompletionMessage = "You finally escaped the Russian Prison and reunited with your family after 10 years of imprisonment";
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
            Invoke("LoadLevel1", 6f);
        }
        else if (levelIndex == 2)
        {
            currentMessage = Level2Message;
            Invoke("LoadLevel2", 5f);
        }
        else if (levelIndex == 3)
        {
            currentMessage = Level3Message;
            Invoke("LoadLevel3", 7f);
        }
        else if (levelIndex == 4)
        {
            currentMessage = CompletionMessage;
            Invoke("LoadMainMenu", 10f);
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
    void LoadLevel1()
    {
        SceneManager.LoadScene("Level1-Caleb");
    }
    void LoadLevel2()
    {
        SceneManager.LoadScene("Level2-Jacqueline");
    }
    void LoadLevel3()
    {
        SceneManager.LoadScene("Level3-Daniel");
    }
    void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}