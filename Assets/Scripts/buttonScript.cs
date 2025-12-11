using UnityEngine;


[RequireComponent(typeof(Animator))]
public class buttonScript : MonoBehaviour
{

    public GameObject wall;
    private Animator _animator;
    private GameObject _rbody;


    SewerCameraScript _cameraScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _cameraScript = Camera.main.GetComponent<SewerCameraScript>();
        _animator = GetComponent<Animator>();
        _animator.SetBool("isPressed", false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && wall.activeSelf)
        {
            _animator.SetBool("isPressed", true);
            PlayerPrefs.SetInt("PlayerImmune", 1);
            _cameraScript.publicPanTo(wall.transform.position, wall);
        }
    }
}

