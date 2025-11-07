using UnityEngine;

public class EndingSceneManagerScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMenuPress() {
        Debug.Log("Menu Pressed");
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
