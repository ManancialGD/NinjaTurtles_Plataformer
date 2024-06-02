using System;
using UnityEngine;

public class ShooterAim : MonoBehaviour
{
    // Reference to the RotateArm component
    private RotateArm rotateArm;

    // Bullet prefab to spawn
    public EnemyBullet prefabToSpawn;

    // Gravity scale of the bullet prefab
    private float prefabGravityScale;

    // Flag to display trajectory in the editor
    [SerializeField] private bool displayTrajectory;

    // Reference to the target transform
    private Transform target;

    // Flag to minimize time or maximize distance in trajectory calculation
    [SerializeField] public bool minimizeTime;

    void Awake()
    {
        // Find and assign the target (Leo) at the start
        target = FindObjectOfType<LeoMovement>().transform;

        // Get the RotateArm component from the parent
        rotateArm = GetComponentInParent<RotateArm>();

        // Get the gravity scale of the bullet prefab's Rigidbody2D component
        prefabGravityScale = prefabToSpawn.GetComponent<Rigidbody2D>().gravityScale;
    }

    void Update()
    {
        // Check for the input to shoot (KeyCode 5)
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            // Compute and set the velocity for the shot
            if (ComputeVelocity(transform.position, target.position, prefabToSpawn.speed, Physics2D.gravity.y, minimizeTime, out Vector2 vel))
            {
                // Instantiate the bullet prefab and set its velocity
                var newShot = Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
                newShot.SetVelocity(vel);

                // Start the recoil coroutine
                StartCoroutine(rotateArm.Recoil());
            }
            else
            {
                Debug.LogWarning("Impossible to hit target!");
            }
        }
    }

    private void FixedUpdate()
    {
        // Compute the velocity continuously in FixedUpdate
        ComputeVelocity(transform.position, target.position, prefabToSpawn.speed, Physics2D.gravity.y, minimizeTime, out Vector2 vel);
    }

    /// <summary>
    /// Computes the angle needed to hit the target and translates it into the initial velocity vector.
    ///
    /// This piece of code was created by professor Diogo
    /// If you want to know more checkout the link:
    /// https://github.com/DiogoDeAndrade/projetil
    /// </summary>
    /// <param name="srcPos">The source position.</param>
    /// <param name="targetPos">The target position.</param>
    /// <param name="speed">The initial speed of the projectile.</param>
    /// <param name="gravity">The gravitational acceleration.</param>
    /// <param name="minimizeTime">Flag to minimize time or maximize distance.</param>
    /// <param name="shotVelocity">The computed initial velocity vector.</param>
    /// <returns>True if a valid velocity is found; false otherwise.</returns>
    public bool ComputeVelocity(Vector3 srcPos, Vector3 targetPos, float speed, float gravity, bool minimizeTime, out Vector2 shotVelocity)
    {
        shotVelocity = Vector2.zero;

        float invX = 1.0f;
        float deltaX = targetPos.x - srcPos.x;
        if (deltaX < 0.0f)
        {
            deltaX = -deltaX;
            invX = -1.0f;
        }
        float tmp = gravity * (deltaX * deltaX) / (2.0f * speed * speed) * prefabGravityScale;
        float a = tmp;
        float b = deltaX;
        float c = (srcPos.y - targetPos.y + tmp);

        if (Mathf.Abs(a) < 1e-6)
        {
            return false;
        }

        float D = b * b - 4 * c * a;
        if (D < 0.0f)
        {
            return false;
        }
        D = Mathf.Sqrt(D);

        float theta1 = Mathf.Atan((-b - D) / (2.0f * a));
        float theta2 = Mathf.Atan((-b + D) / (2.0f * a));

        float t1 = deltaX / (speed * Mathf.Cos(theta1));
        float t2 = deltaX / (speed * Mathf.Cos(theta2));

        float theta = 0.0f;
        if (t1 < 0.0f)
        {
            if (t2 < 0.0f)
            {
                return false;
            }
            else
            {
                theta = theta2;
            }
        }
        else
        {
            if (t2 < 0.0f)
            {
                theta = theta1;
            }
            else
            {
                if (minimizeTime)
                {
                    if (t1 < t2) theta = theta1;
                    else theta = theta2;
                }
                else
                {
                    if (t1 < t2) theta = theta2;
                    else theta = theta1;
                }
            }
        }

        shotVelocity = new Vector2(invX * speed * Mathf.Cos(theta), speed * Mathf.Sin(theta));

        // Set the rotation angle of the RotateArm
        rotateArm.SetRotationAngle(theta);

        return true;
    }

    void OnDrawGizmos()
    {
        if (!displayTrajectory) return;

        // Initialize target if null
        if (target == null)
        {
            target = FindObjectOfType<LeoMovement>()?.transform;
            if (target == null) return;
        }

        if (ComputeVelocity(transform.position, target.position, prefabToSpawn.speed, Physics2D.gravity.y, minimizeTime, out Vector2 vel))
        {
            GizmoSimulation(Color.cyan, vel);
        }
    }

    /// <summary>
    /// Simulates and draws the projectile's trajectory in the editor.
    /// </summary>
    /// <param name="color">Color of the trajectory line.</param>
    /// <param name="vel">Initial velocity of the projectile.</param>
    void GizmoSimulation(Color color, Vector2 vel)
    {
        float timeStep = 0.01f;
        float t = 0.0f;
        Vector2 startPos = transform.position;
        Vector2 prevPos = transform.position;

        Gizmos.color = color;

        while (t < 5.0f)
        {
            Vector2 pos = startPos + vel * t + 0.5f * Physics2D.gravity * t * t * prefabGravityScale;

            Gizmos.DrawLine(prevPos, pos);

            prevPos = pos;

            t += timeStep;
        }
    }
}
