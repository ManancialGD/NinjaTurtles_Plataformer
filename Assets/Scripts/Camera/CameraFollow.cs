using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform target;
    private Vector3 velocity = Vector3.zero;

    [Range(0, 1)]
    public float smoothTime = 0.3f;
    public Vector3 positionOffset;
    [Header("Axis Limitation")]
    public Vector2 xLimit; // Vector2(Xmin, Xmax)
    public Vector2 yLimit; // Vector2(Ymin, Ymax)

    Camera cam;

    private void Update()
    {
        if (target == null) return;

        FollowTarget(target);
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    /// <summary>
    /// This will make the camera not go beyond the limits and will follow the target
    /// position with a smooth value.
    /// </summary>
    private void FollowTarget(Transform target)
    {
        Vector3 targetPosition = target.position + positionOffset;
        
        // Clamp the target position within the specified limits
        float clampedX = Mathf.Clamp(targetPosition.x, xLimit.x, xLimit.y);
        float clampedY = Mathf.Clamp(targetPosition.y, yLimit.x, yLimit.y);
        float clampedZ = transform.position.z; // Maintain current z-position
        
        Vector3 clampedPosition = new Vector3(clampedX, clampedY, clampedZ);

        // Smoothly move the camera towards the clamped position
        transform.position = Vector3.SmoothDamp(transform.position, clampedPosition, ref velocity, smoothTime);
    }

    /// <summary>
    /// This will draw a box with the limits on the scene.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (cam == null)
        {
            cam = GetComponent<Camera>();
            if (cam == null) return;
        }

        Gizmos.color = Color.red;

        // Calculate the center and size of the rectangle, adjusted for camera size
        float camHeight = cam.orthographicSize * 2;
        float camWidth = camHeight * cam.aspect;

        Vector3 center = new Vector3((xLimit.x + xLimit.y) / 2, (yLimit.x + yLimit.y) / 2, transform.position.z);
        Vector3 size = new Vector3(xLimit.y - xLimit.x + camWidth, yLimit.y - yLimit.x + camHeight, 1);

        // Draw the wireframe cube
        Gizmos.DrawWireCube(center, size);
    }
}
