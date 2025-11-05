using UnityEngine;

public class CarScript : MonoBehaviour
{

    Level3SceneManagerScript _manager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _manager = FindAnyObjectByType<Level3SceneManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
           _manager.playerWon();
        }
    }
}

