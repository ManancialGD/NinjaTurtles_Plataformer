using System.Collections;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    DetectLeo leodetection;
    ShooterAim aim;
    RotateArm arm;
    Transform target;

    private bool waitingToShoot;

    private void Awake()
    {
        leodetection = GetComponent<DetectLeo>();
        if (leodetection == null)
        {
            Debug.LogError("DetectLeo component not found on " + gameObject.name);
        }

        arm = GetComponentInChildren<RotateArm>();
        if (arm == null)
        {
            Debug.LogError("RotateArm component not found on " + gameObject.name);
        }

        aim = arm.GetComponentInChildren<ShooterAim>();
        if (aim == null)
        {
            Debug.LogError("ShooterAim component not found on " + gameObject.name);
        }

        var leoMovement = FindObjectOfType<LeoMovement>();
        if (leoMovement != null)
        {
            target = leoMovement.transform;
        }
        else
        {
            Debug.LogError("LeoMovement object not found in the scene");
        }
    }

    private void FixedUpdate()
    {
        // Return early if any required component is missing
        if (leodetection == null || aim == null || target == null || arm == null)
        {
            return;
        }

        if (leodetection.leoDetected)
        {
            // Always aim towards the target if detected
            aim.ComputeVelocity(aim.transform.position, target.position, aim.prefabToSpawn.speed, Physics2D.gravity.y, aim.minimizeTime, out Vector2 vel);
            
            // Set the rotation angle for the arm based on the velocity
            float angle = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg;
            arm.SetRotationAngle(angle);

            // Start shooting if not already waiting to shoot
            if (!waitingToShoot)
            {
                StartCoroutine(ShootAfterDelay(2f, vel));
            }
        }
        else
        {
            arm.SetRotationAngle(0);
        }
    }

    private IEnumerator ShootAfterDelay(float delay, Vector2 vel)
    {
        waitingToShoot = true;

        // Instantiate the bullet prefab and set its velocity
        var newShot = Instantiate(aim.prefabToSpawn, aim.transform.position, Quaternion.identity);
        newShot.SetVelocity(vel);

        // Start the recoil coroutine
        StartCoroutine(arm.Recoil());

        yield return new WaitForSeconds(delay);
        waitingToShoot = false;
    }
}