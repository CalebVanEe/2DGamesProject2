using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadingSceneManagerScript : MonoBehaviour
{
    public string Level1FirstMessage = "Your name is Dimitri. You have been wrongfully imprisoned in a Russian prison for the past 10 years";
    public string Level1SecondMessage = "You have finally had enough and decide to escape, but it won't be easy";
    public string Level1ThirdMessage = "You are in the depths of the prison in the torture chamber. You must find your way out to survive";
    public string Level2Message = "You've escaped the Torture Chamber, and find yourself locked in the Warden's Office. Find the 2 keys to unlock the door";
    public string Level3Message = "You've made your way out to the prison yard. The guards have discovered your absence and are looking for you. Escape or die";
    public string CompletionMessage = "You finally escaped the Russian Prison and reunited with your family after 10 years of imprisonment";
    public int levelIndex;
    public TMP_Text messageText;
    public string currentLevelName;
    void Start()
    {
        DisplayMessage();
        PlayerPrefs.SetString("CurrentLevel", currentLevelName);
    }

    void DisplayMessage()
    {
        if (levelIndex == 1)
        {

            StartCoroutine(DisplayLevel1Messages());
        }
        else if (levelIndex == 2)
        {

            StartCoroutine(TypeText(Level2Message));
            Invoke("LoadLevel2", 7f);
        }
        else if (levelIndex == 3)
        {
            StartCoroutine(TypeText(Level3Message));
            Invoke("LoadLevel3", 8f);
        }
        else if (levelIndex == 4)
        {
            StartCoroutine(TypeText(CompletionMessage));
            Invoke("WinningScene", 10f);
        }
    }

    IEnumerator DisplayLevel1Messages()
    {
        // Display first message
        yield return StartCoroutine(TypeText(Level1FirstMessage));
        yield return new WaitForSeconds(2f); // 1 second delay

        // Display second message
        yield return StartCoroutine(TypeText(Level1SecondMessage));
        yield return new WaitForSeconds(2f); // 1 second delay

        // Display third message
        yield return StartCoroutine(TypeText(Level1ThirdMessage));

        // Wait a bit after the last message, then load the level
        yield return new WaitForSeconds(2f);
        LoadLevel1();
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
        currentLevelName = "Level1-Caleb";
        PlayerPrefs.SetString("CurrentLevel", currentLevelName);
        SceneManager.LoadScene("Level1-Caleb");
    }

    void LoadLevel2()
    {
        currentLevelName = "Level2-Jacqueline";
        PlayerPrefs.SetString("CurrentLevel", currentLevelName);
        SceneManager.LoadScene("Level2-Jacqueline");
    }

    void LoadLevel3()
    {

        currentLevelName = "Level3-Daniel";
        PlayerPrefs.SetString("CurrentLevel", currentLevelName);
        SceneManager.LoadScene("Level3-Daniel");
    }

    void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void WinningScene()
    {
        SceneManager.LoadScene("WinningScene");
    }   
}