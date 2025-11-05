using UnityEngine;

public class tutorialKeyScript : MonoBehaviour
{
    tutorialSceneManagerScript scene;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scene = FindAnyObjectByType<tutorialSceneManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        scene.Finding(gameObject);
    }
}
