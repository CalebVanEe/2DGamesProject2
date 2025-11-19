using UnityEngine;

public class SlidingDoorScript : MonoBehaviour
{
    FloorLevelScript script;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        script = FindAnyObjectByType<FloorLevelScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //script.hitDoor();
    }
}
