using UnityEngine;

public class AxePivotScript : MonoBehaviour
{
    public float maxAngleDeflection = 60f;
    public float speedOfPendulum = 1f;
    public float smoothTime = 1f;

    private float _currentAngleDeflection;
    private float _velocity;
    private Vector3 _startPosition;

    void Start()
    {
        _currentAngleDeflection = maxAngleDeflection;
        _startPosition = transform.position;
    }

    void Update()
    {
        // Smoothly transition current angle to target max angle
        _currentAngleDeflection = Mathf.SmoothDamp(
            _currentAngleDeflection,
            maxAngleDeflection,
            ref _velocity,
            smoothTime
        );

        // Calculate the swing angle using sine wave
        float angle = _currentAngleDeflection * Mathf.Sin(Time.time * speedOfPendulum);

        // Reset position and rotation
        transform.position = _startPosition;
        transform.rotation = Quaternion.identity;

        // Rotate around the pivot point
        transform.RotateAround(_startPosition, Vector3.forward, angle);
    }
}