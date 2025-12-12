using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class bulletScript : MonoBehaviour
{
    Rigidbody2D _rbody;
    SewerSceneManager _manager;
    public GameObject knockedOutSprite;
    private SewerCameraScript sewerCameraScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _manager = FindObjectOfType<SewerSceneManager>();
        _rbody = GetComponent<Rigidbody2D>();
        sewerCameraScript = GameObject.Find("Main Camera").GetComponent<SewerCameraScript>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (_rbody != null && _rbody.linearVelocity.magnitude < 1)
        {
            gameObject.SetActive(false);
        }
    }
    private void onBecomeInvisible()
    {
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (PlayerPrefs.GetInt("PlayerImmune") == 0 && collision.gameObject.CompareTag("Player")) 
        {
            Invoke("KillPlayer", 2f);

            Destroy(collision.gameObject);
            Vector2 position = collision.transform.position;
            GameObject deadPlayer = Instantiate(knockedOutSprite, position, Quaternion.identity);
            deadPlayer.GetComponent<Rigidbody2D>().angularVelocity = 5f;
            sewerCameraScript.SetPlayer(deadPlayer);
            PlayerPrefs.SetString("KillMessage", "You got Shot");
        }
        gameObject.SetActive(false);

    }
    private void KillPlayer()
    {
        _manager.playerCaught();
    }
}
