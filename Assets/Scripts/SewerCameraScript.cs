using UnityEngine;
using System.Threading.Tasks;

using System.Collections;
using System.Collections.Generic;
public class SewerCameraScript : MonoBehaviour
{
    public GameObject player;
    public Vector3 offset = new Vector3(0, 0, -10);


    public float leftDeadzonePercent = 0.3f;
    public float rightDeadzonePercent = 0.5f;


    public float bottomDeadzonePercent = 0.35f;
    public float topDeadzonePercent = 0.65f;

    public float cameraSmoothSpeed = 8f;

    private Vector3 targetPosition;



    public GameObject[] grates;
    private bool shouldPan;
    private int currentGrateIndex = 0;

    public GameObject wall1;
    public GameObject wall2;
    public GameObject wall3;
    public GameObject wall4;

    private bool track;
    // Start is called once before 
    void Start()
    {
        track = true;
        grates = new GameObject[] { wall1, wall2, wall3, wall4 };
        shouldPan = true;
        if (PlayerPrefs.GetInt("SewerIntroSeen") == 0)
        {
            track = false;
            StartCoroutine(showLevel(grates));
        }

        if (player != null)
        {
            targetPosition = new Vector3(player.transform.position.x + offset.x, player.transform.position.y + offset.y, offset.z);
            transform.position = targetPosition;
        }

    }
    Vector3 startPosition;
    private IEnumerator showLevel(GameObject[] grates)
    {
        startPosition = transform.position;


        for (int i = 0; i < grates.Length; i++)
        {

            Vector3 gratePosition = new Vector3(grates[i].transform.position.x, grates[i].transform.position.y, -10);
            yield return StartCoroutine(panTo(gratePosition));
        }
        transform.position = startPosition;
        track = true;
        PlayerPrefs.SetInt("SewerIntroSeen", 1);

    }

    public float panSpeed;
    private IEnumerator panTo(Vector3 gratePosition)
    {
        while (Vector3.Distance(transform.position, gratePosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, gratePosition, panSpeed * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(1f);

        transform.position = gratePosition;
    }
    void LateUpdate()
    {
        if (track)
        {
            if (player == null) return;

            // Get camera viewport bounds in world space
            Camera cam = GetComponent<Camera>();
            float cameraHeight = cam.orthographicSize * 2f;
            float cameraWidth = cameraHeight * cam.aspect;

            // Calculate deadzone boundaries in world space
            float leftBoundary = targetPosition.x - (cameraWidth * 0.5f) + (cameraWidth * leftDeadzonePercent);
            float rightBoundary = targetPosition.x - (cameraWidth * 0.5f) + (cameraWidth * rightDeadzonePercent);
            float bottomBoundary = targetPosition.y - (cameraHeight * 0.5f) + (cameraHeight * bottomDeadzonePercent);
            float topBoundary = targetPosition.y - (cameraHeight * 0.5f) + (cameraHeight * topDeadzonePercent);

            // Get player position
            float playerX = player.transform.position.x;
            float playerY = player.transform.position.y;

            // Update target X position based on player position relative to deadzone
            if (playerX < leftBoundary)
            {
                // Player is too far left, move camera left
                targetPosition.x += (playerX - leftBoundary);
            }
            else if (playerX > rightBoundary)
            {
                // Player is too far right, move camera right
                targetPosition.x += (playerX - rightBoundary);
            }

            // Update target Y position based on player position relative to deadzone
            if (playerY < bottomBoundary)
            {
                // Player is too far down, move camera down
                targetPosition.y += (playerY - bottomBoundary);
            }
            else if (playerY > topBoundary)
            {
                // Player is too far up, move camera up
                targetPosition.y += (playerY - topBoundary);
            }

            // Apply offset
            targetPosition.x += offset.x;
            targetPosition.y += offset.y;
            targetPosition.z = offset.z;

            // Smoothly move camera to target position
            transform.position = Vector3.Lerp(transform.position, targetPosition, cameraSmoothSpeed * Time.deltaTime);

            // Remove the offset for the next frame
            targetPosition.x -= offset.x;
            targetPosition.y -= offset.y;
        }
    }

    public void publicPanTo(Vector3 panToPosition, GameObject grate) {
        track = false;
        startPosition = transform.position;

        StartCoroutine(panToGrate(panToPosition, grate));
    }

    private IEnumerator panToGrate(Vector3 gratePosition, GameObject grate)
    {
        gratePosition.z = offset.z;
        while (Vector3.Distance(transform.position, gratePosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, gratePosition, panSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = gratePosition;
        grate.SetActive(false);
        
        yield return new WaitForSeconds(1f);

        transform.position = startPosition;
        track = true;
        PlayerPrefs.SetInt("PlayerImmune", 0);
    }
    public void SetPlayer(GameObject newPlayer)
    {
        player = newPlayer;
        if (player != null)
        {
            targetPosition = new Vector3(player.transform.position.x + offset.x, player.transform.position.y + offset.y, offset.z);
            transform.position = targetPosition;
        }
    }
}