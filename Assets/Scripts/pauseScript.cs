using UnityEngine;
using UnityEngine.SceneManagement;

public class pauseScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        string lastScene = PlayerPrefs.GetString("CurrentLevel");
        SceneManager.LoadScene(lastScene);
    }
}
