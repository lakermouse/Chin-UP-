using UnityEngine;

/// <summary>
/// Lets a camera fly above the stadium in a circular pattern.
/// </summary>
public class StadiumCamera : MonoBehaviour
{
    // The radius of the flying circle.
    public float radius = 55.0f;

    // The height (y coordinate) of the camera.
    public float height = 25.0f;

    // The camera's speed in [°/s]
    public float speed = 1.0f;

    private float angle;

    private void Start()
    {
        angle = 0.0f;
    }

    private void Update()
    {
        float x = radius * Mathf.Cos(angle);
        float z = radius * Mathf.Sin(angle);

        transform.position = new Vector3(x, height, z);
        transform.LookAt(Vector3.zero);

        angle += speed * Time.deltaTime;
    }
}