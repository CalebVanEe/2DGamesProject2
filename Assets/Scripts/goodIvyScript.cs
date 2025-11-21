using UnityEngine;

public class goodIvyScript : MonoBehaviour
{
    FloorLevelScript floorLevelScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        floorLevelScript = FindAnyObjectByType<FloorLevelScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        floorLevelScript.hitGoodIvy(gameObject);
    }
}
