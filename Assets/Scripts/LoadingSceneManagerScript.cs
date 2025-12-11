using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadingSceneManagerScript : MonoBehaviour
{
    private string Level1FirstMessage = "Your name is Dimitri. You have been wrongfully imprisoned in a Russian prison for the past 10 years";
    private string Level1SecondMessage = "You have finally had enough and decide to escape, but it won't be easy";
    private string Level1ThirdMessage = "You are in the depths of the prison in the torture chamber. You must find your way out to survive";
    private string Level2Message = "You found yourself trapped behind a security door, find the combination to escape";
    private string Level3Message = "You've made your way to the cell block. It is past curfew, avoid the guards and find a way out";
    private string Level4Message = "You've escaped the cell block, and find yourself locked in the Warden's Office. Find the 2 keys to unlock the door";
    private string Level5Message = "You stumbled into the prison armory, the guards here are heavily armed. Run for your life";
    private string Level6Message = "You've made your way out to the prison yard. All the guards have been notified of your absence and are looking for you. Escape or die";
    private string CompletionMessage = "You finally escaped the Russian Prison and reunited with your family after 10 years of imprisonment";
    public int levelIndex;
    public TMP_Text messageText;
    public string currentLevelName;
    private string[] loadingMessages;
    void Start()
    {
        loadingMessages = new string[]
        {
            Level2Message,
            Level3Message,
            Level4Message,
            Level5Message,
            Level6Message,
        };
        DisplayMessage();
        PlayerPrefs.SetString("CurrentLevel", currentLevelName);
    }

    void DisplayMessage()
    {
        if (levelIndex == 1)
        {
            StartCoroutine(DisplayLevel1Messages());
        }
        else if (levelIndex < 7)
        {
            StartCoroutine(TypeText(loadingMessages[levelIndex-2]));
            Invoke("LoadLevel" + levelIndex, 7f);
        }
        else if (levelIndex == 7)
        {
            StartCoroutine(TypeText(CompletionMessage));
            Invoke("WinningScene", 10f);
        }
    }
    public void SkipMessage()
    {
        StopAllCoroutines();
        Invoke("LoadLevel" + levelIndex, 0.1f);
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
        currentLevelName = "Level1-TortureChamber";
        PlayerPrefs.SetString("CurrentLevel", currentLevelName);
        SceneManager.LoadScene("Level1-TortureChamber");
    }

    void LoadLevel2()
    {
        currentLevelName = "Level2-SecurityDoor";
        PlayerPrefs.SetString("CurrentLevel", currentLevelName);
        SceneManager.LoadScene("Level2-SecurityDoor");
    }

    void LoadLevel3()
    {

        currentLevelName = "Level3-CellBlock";
        PlayerPrefs.SetString("CurrentLevel", currentLevelName);
        SceneManager.LoadScene("Level3-CellBlock");
    }

    void LoadLevel4()
    {

        currentLevelName = "Level4-WardenOffice";
        PlayerPrefs.SetString("CurrentLevel", currentLevelName);
        SceneManager.LoadScene("Level4-WardenOffice");
    }

    void LoadLevel5()
    {

        currentLevelName = "Level5-Armory";
        PlayerPrefs.SetInt("SewerIntroSeen", 0);
        PlayerPrefs.SetString("CurrentLevel", currentLevelName);
        SceneManager.LoadScene("Level5-Armory");
    }

    void LoadLevel6()
    {

        currentLevelName = "Level6-PrisonYard";
        PlayerPrefs.SetString("CurrentLevel", currentLevelName);
        SceneManager.LoadScene("Level6-PrisonYard");
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