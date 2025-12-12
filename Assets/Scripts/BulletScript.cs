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
        gameObject.SetActive(false);
        if (PlayerPrefs.GetInt("PlayerImmune") == 0 && collision.gameObject.CompareTag("Player")) 
        {
            GameObject deadPlayer = Instantiate(knockedOutSprite, collision.transform.position, Quaternion.identity);
            deadPlayer.GetComponent<Rigidbody2D>().angularVelocity = 5f;
            sewerCameraScript.SetPlayer(deadPlayer);
            Destroy(collision.gameObject);
            PlayerPrefs.SetString("KillMessage", "You got Shot");
            Invoke("KillPlayer", 2f);
        }
    }
    private void KillPlayer()
    {
        _manager.playerCaught();
    }
}
