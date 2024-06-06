using System.Collections;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    DetectLeo leodetection;
    ShooterAim aim;
    RotateArm arm;
    Transform target;
    EnemyHP hp;

    [Header("Times")]
    private float cooldownTime = 2;

    [Space]

    [Header("Bools")]
    private bool waitingToShoot;
    private bool firstDetected;

    private void Awake()
    {
        hp = GetComponent<EnemyHP>();
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
        firstDetected = true;
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
            if (aim.ComputeVelocity(aim.transform.position, target.position, aim.prefabToSpawn.speed, Physics2D.gravity.y, aim.minimizeTime, out Vector2 vel, out float theta))
            {
                arm.SetRotationAngle(theta);

                if (firstDetected)
                {
                    StartCoroutine(ShootAfterDelay(cooldownTime));
                    firstDetected = false;
                }
                // Start shooting if not already waiting to shoot
                if (!waitingToShoot && !hp.IsStunned)
                {
                    StartCoroutine(ShootAfterDelay(cooldownTime));

                    // Instantiate the bullet prefab and set its velocity
                    var newShot = Instantiate(aim.prefabToSpawn, aim.transform.position, Quaternion.identity);
                    newShot.SetVelocity(vel);

                    // Start the recoil coroutine
                    StartCoroutine(arm.Recoil());
                }
            }
        }
        else
        {
            arm.SetRotationAngle(0);
            firstDetected = true;
        }
    }

    private IEnumerator ShootAfterDelay(float delay)
    {
        waitingToShoot = true;

        yield return new WaitForSeconds(delay);

        waitingToShoot = false;
    }
}