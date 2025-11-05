using UnityEngine;

public class EndingSceneManagerScript : MonoBehaviour
{
    public TMPro.TMP_Text score; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        score.text = "Congratulations! \n you escaped the prison in " + PlayerPrefs.GetFloat("LevelTime") + " seconds";   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
