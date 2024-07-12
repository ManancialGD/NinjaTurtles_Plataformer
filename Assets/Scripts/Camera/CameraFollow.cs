using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform target;
    private Rigidbody2D rb;
    private Vector3 velocity = Vector3.zero;

    [SerializeField] private bool cameraShaking;
    [SerializeField] private float cameraShakeCooldown = 1;
    [SerializeField] private float cameraShakeResistence = 2;

    [Range(0, 1)]
    public float smoothTime = 0.3f;
    public Vector3 positionOffset;
    [Header("Axis Limitation")]
    public Vector2 xLimit; // Vector2(Xmin, Xmax)
    public Vector2 yLimit; // Vector2(Ymin, Ymax)

    [SerializeField] Color debugColour = Color.blue;

    private Camera cam;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
    }

    private void Update()
    {
        if (target != null)
        {
            FollowTarget(target);
        }
    }

    private Vector3 ClampCamera(Vector3 targetPosition)
    {
        float camHeight = cam.orthographicSize;
        float camWidth = cam.orthographicSize * cam.aspect;

        float minX = xLimit.x + camWidth;
        float maxX = xLimit.y - camWidth;

        float minY = yLimit.x + camHeight;
        float maxY = yLimit.y - camHeight;

        float newX = Mathf.Clamp(targetPosition.x, minX, maxX);
        float newY = Mathf.Clamp(targetPosition.y, minY, maxY);

        return new Vector3(newX, newY, targetPosition.z);
    }

    private void OnDrawGizmos()
    {
        if (cam == null)
        {
            cam = Camera.main;
            if (cam == null) return;
        }

        Gizmos.color = debugColour;

        // Draw the camera limits as wireframe boxes
        float camHeight = cam.orthographicSize;
        float camWidth = cam.orthographicSize * cam.aspect;

        Vector3 center = new Vector3((xLimit.x + xLimit.y) / 2f, (yLimit.x + yLimit.y) / 2f, 0f);
        Vector3 size = new Vector3(xLimit.y - xLimit.x, yLimit.y - yLimit.x, 0f);

        Gizmos.DrawWireCube(center, size);
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

        Vector3 clampedPosition = ClampCamera(targetPosition);

        // Smoothly move the camera towards the clamped position
        transform.position = Vector3.SmoothDamp(transform.position, clampedPosition, ref velocity, smoothTime);
    }

    public void CameraShake(float shakeTime, float shakeInterval, float shakeResistence, Vector2 velocityIntensity)
    {
        if (!cameraShaking)
        {
            cameraShaking = true;
            cameraShakeCooldown = shakeTime;
            cameraShakeResistence = shakeResistence;

            StartCoroutine(SetCameraShakeProperties(0f, shakeInterval, velocityIntensity));
            StartCoroutine(FinishCameraShake(shakeTime));
        }
    }

    public void DamageCameraShake()
    {
        CameraShake(0.15f, 0.05f, 0.99f, new Vector2(8f, 3f));
    }

    private IEnumerator SetCameraShakeProperties(float waitTime, float shakeInterval, Vector2 velocityIntensity)
    {
        yield return new WaitForSeconds(waitTime);

        Debug.Log("Camera Shake started");

        int num = 1;

        Vector2 currentDisplacement = new Vector2(0f, 0f);

        while (cameraShaking)
        {
            if (cameraShakeCooldown <= 0f)
            {
                cameraShaking = false;
                cameraShakeCooldown = 0f;
                break;
            }

            currentDisplacement += num * velocityIntensity;
            rb.velocity += num * velocityIntensity;
            num = -num;
            yield return new WaitForSeconds(shakeInterval);
        }

        rb.velocity -= currentDisplacement;

        Debug.Log("Camera Shake finished");
    }

    private IEnumerator FinishCameraShake(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        cameraShakeCooldown = 0f;
        cameraShaking = false;
    }
}
