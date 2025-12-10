using UnityEngine;

public class BladeScript : MonoBehaviour
{
    private LevelSceneManagerScript levelSceneManagerScript;
    public float spinsPerSecond = 1f;
    private float _rotateZ;

    void Start()
    {
        _rotateZ = 0;
        levelSceneManagerScript = GameObject.Find("LevelSceneManager").GetComponent<LevelSceneManagerScript>();
    }

    void Update()
    {
        _rotateZ += 360f * spinsPerSecond * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, 0, _rotateZ);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            PlayerPrefs.SetString("KillMessage", "ripped apart");
            levelSceneManagerScript.PlayerHit();
    }
}