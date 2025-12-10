using UnityEngine;

public class cameraScript : MonoBehaviour
{
    [Header("Target")]
    public GameObject player;
    public Vector3 offset = new Vector3(0, 0, -10);

    [Header("Horizontal Deadzone")]
    [Range(0f, 0.5f)]
    public float leftDeadzonePercent = 0.3f;
    [Range(0.5f, 1f)]
    public float rightDeadzonePercent = 0.5f;

    [Header("Vertical Deadzone")]
    [Range(0f, 0.5f)]
    public float bottomDeadzonePercent = 0.35f;
    [Range(0.5f, 1f)]
    public float topDeadzonePercent = 0.65f;

    [Header("Camera Smoothing")]
    public float cameraSmoothSpeed = 8f;

    private Vector3 targetPosition;

    void Start()
    {
        if (player != null)
        {
            targetPosition = new Vector3(player.transform.position.x + offset.x, player.transform.position.y + offset.y, offset.z);
            transform.position = targetPosition;
        }
    }

    void LateUpdate()
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