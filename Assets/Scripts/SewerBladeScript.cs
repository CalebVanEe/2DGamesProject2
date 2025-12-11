using UnityEngine;

public class SewerBladeScript : MonoBehaviour
{
    private SewerSceneManager _manager;
    public float spinsPerSecond = 1f;
    private float _rotateZ;

    void Start()
    {
        _rotateZ = 0;
        _manager = FindObjectOfType<SewerSceneManager>();
        ;
    }

    void Update()
    {
        _rotateZ += 360f * spinsPerSecond * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, 0, _rotateZ);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) _manager.playerCaught();
    }
}