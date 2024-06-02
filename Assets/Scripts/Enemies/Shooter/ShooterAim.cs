using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterAim : MonoBehaviour
{
    RotateArm rotateArm;
    public EnemyBullet prefabToSpawn;
    private float prefabGravityScale;
    [SerializeField] private bool displayTrajectory;
    Transform target; 
    [SerializeField] public bool minimizeTime;

    void Awake()
    {
        target = FindObjectOfType<LeoMovement>().transform;
        rotateArm = GetComponentInChildren<RotateArm>();
        if (prefabToSpawn != null)
        {
            prefabGravityScale = prefabToSpawn.GetComponent<Rigidbody2D>().gravityScale;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown("5"))
        {
            // Compute and set the velocity
            if (ComputeVelocity(transform.position, target.position, prefabToSpawn.speed, Physics2D.gravity.y, minimizeTime, out Vector2 vel))
            {
                var newShot = Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
                newShot.SetVelocity(vel);
            }
            else
            {
                Debug.LogWarning("Impossible to hit target!");
            }
        }
    }

    private void FixedUpdate()
    {
        ComputeVelocity(transform.position, target.position, prefabToSpawn.speed, Physics2D.gravity.y, minimizeTime, out Vector2 vel);
    }

    /// <summary>
    /// This Method was made by professor Diogo
    ///
    /// here is the link, if you want to know more about it:
    /// https://github.com/DiogoDeAndrade/projetil
    ///</summary>
    public bool ComputeVelocity(Vector3 srcPos, Vector3 targetPos, float speed, float gravity, bool minimizeTime, out Vector2 shotVelocty)
    {
        shotVelocty = Vector2.zero;

        float invX = 1.0f;
        float deltaX = targetPos.x - srcPos.x;
        if (deltaX < 0.0f)
        {
            deltaX = -deltaX;
            invX = -1.0f;
        }
        float tmp = gravity * (deltaX * deltaX) / (2.0f * speed * speed) * 6;
        float a = tmp;
        float b = deltaX;
        float c = (srcPos.y - targetPos.y + tmp);

        if (Mathf.Abs(a) < 1e-6)
        {
            // Equation is unsolveable
            return false;
        }

        float D = b * b - 4 * c * a;
        if (D < 0.0f)
        {
            // Equation is unsolveable
            return false;
        }
        D = Mathf.Sqrt(D);

        // Two solutions
        float theta1 = Mathf.Atan((-b - D) / (2.0f * a));
        float theta2 = Mathf.Atan((-b + D) / (2.0f * a));

        // Find the times for impact
        float t1 = deltaX / (speed * Mathf.Cos(theta1));
        float t2 = deltaX / (speed * Mathf.Cos(theta2));

        float theta = 0.0f;
        if (t1 < 0.0f)
        {
            if (t2 < 0.0f)
            {
                // Equation is unsolveable
                return false;
            }
            else
            {
                // Only one valid solution
                theta = theta2;
            }
        }
        else
        {
            if (t2 < 0.0f)
            {
                // Only one valid solution
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

        shotVelocty = new Vector2(invX * speed * Mathf.Cos(theta), speed * Mathf.Sin(theta));

        rotateArm.SetRotationAngle(theta);

        return true;
    }

    void OnDrawGizmos()
    {
        if (!displayTrajectory) return;

        if (ComputeVelocity(transform.position, target.position, prefabToSpawn.speed, Physics2D.gravity.y, minimizeTime, out Vector2 vel))
        {
            GizmoSimulation(Color.cyan, vel);
        }
    }

    void GizmoSimulation(Color color, Vector2 vel)
    {
        float timeStep = 0.01f;
        float t = 0.0f;
        Vector2 startPos = transform.position;
        Vector2 prevPos = transform.position;

        Gizmos.color = color;

        while (t < 5.0f)
        {
            Vector2 pos = startPos + vel * t + 0.5f * Physics2D.gravity * t * t;

            Gizmos.DrawLine(prevPos, pos);

            prevPos = pos;

            t += timeStep;
        }
    }
}