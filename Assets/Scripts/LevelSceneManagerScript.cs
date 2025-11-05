using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelSceneManagerScript : MonoBehaviour
{
    public Vector3 Level1Spawn = new Vector3(-8.8f, -3.88f, 0);
    public Vector3 Door1Location = new Vector3(153f, 13, 0);
    public playerScript player;
    public GameObject door;
    private Rigidbody2D _playerbody;
    private bool level1KeyCollected;
    private bool openingDoor;
    public int level;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerbody = player.GetComponent<Rigidbody2D>();
        openingDoor = false;
        level1KeyCollected = false;
        door.GetComponent<Animator>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void RespawnLevel()
    {
        if (level == 1)
            player.transform.position = Level1Spawn;
        _playerbody.linearVelocity = Vector2.zero;
    }
    public void PlayerHit()
    {
        if (level == 1)
            player.transform.position = Level1Spawn;
        _playerbody.linearVelocity = Vector2.zero;
    }
    public void OpenAttempt(bool open)
    {
        openingDoor = open;
    }
    public bool GetOpenAttempt()
    {
        return openingDoor;
    }
    public void LoadLevel(int level)
    {
        if (level == 1)
        {
            player.transform.position = Door1Location;
            _playerbody.linearVelocity = Vector2.zero;
            Level1Spawn = Door1Location;
        }
    }
    public void LevelCompleted()
    {
        if (level == 1)
        {
            if (!level1KeyCollected)
            {
                return;
            }
            else
            {
                door.GetComponent<Animator>().enabled = true;
                Invoke("NextScene", 2);
            }
        }
    }
    private void NextScene()
    {
        SceneManager.LoadScene("Level2LoadingScreen");
    }
    public void KeyCollected()
    {
        if (level == 1)
        {
            level1KeyCollected = true;
        }
    }
}