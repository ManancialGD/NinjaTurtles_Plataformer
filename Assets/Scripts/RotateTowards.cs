using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RotateTowards : MonoBehaviour
{
    Player playerScript; // Reference to the Player script
    public float rotationSpeed;
    public float timeToFix;
    public GameObject thisEnemy;
    Enemy_Ranger rangerScript;
    SuspectScript suspectScript;
    EnemyShoot enemyShoot;
    NativeInfo native;
    Transform target;

    private void Start()
    {
        target = FindObjectOfType<Player>().transform;
        enemyShoot = FindObjectOfType<EnemyShoot>();
        native = FindObjectOfType<NativeInfo>();
        playerScript = native.GetPlayerObj(native.currentPlayerID).GetComponent<Player>();
        rangerScript = GetComponentInParent<Enemy_Ranger>();

        if (playerScript == null)
        {
            Debug.LogError("Player script not found.");
        }
        suspectScript = GetComponentInParent<SuspectScript>();
        Debug.Log("suspectScript = " + suspectScript);
    }

    private void FixedUpdate()
    {
        ComputeVelocity(transform.position, target.position, enemyShoot.speed, Physics2D.gravity.y, false);
    }

    private void ComputeVelocity(Vector3 srcPos, Vector3 targetPos, float speed, float gravity, bool minimizeTime)
    {
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
            return;
        }

        float D = b * b - 4 * c * a;
        if (D < 0.0f)
        {
            // Equation is unsolveable
            return;
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
                return;
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

        SetRotationAngle(theta);
    }

    public void SetRotationAngle(float theta)
    {
        float angle = theta * Mathf.Rad2Deg; // Convert from radians to degrees

        // If the player's position is less than the game object's position, set the angle to negative
        if (target.position.x < gameObject.transform.position.x)
        {
            angle = -angle;
        }

        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * rotationSpeed);
    }
}
