using System.Collections;
using System.Collections.Generic;
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
        aim = GetComponent<ShooterAim>();
        target = GetComponent<LeoMovement>().transform;
    }
    private void FixedUpdate()
    {
        if (leodetection.leoDetected)
        {
            aim.ComputeVelocity(transform.position, target.position, aim.prefabToSpawn.speed, Physics2D.gravity.y, aim.minimizeTime, out Vector2 vel);
        }
        else arm.SetRotationAngle(0);

        if (leodetection.leoDetected && !waitingToShoot)
        {
            StartCoroutine(WaitToShoot());
            waitingToShoot = true;
        }
        else if (!waitingToShoot)
        {
            // Compute and set the velocity for the shot
            if (aim.ComputeVelocity(transform.position, target.position, aim.prefabToSpawn.speed, Physics2D.gravity.y, aim.minimizeTime, out Vector2 vel))
            {
                // Instantiate the bullet prefab and set its velocity
                var newShot = Instantiate(aim.prefabToSpawn, transform.position, Quaternion.identity);
                newShot.SetVelocity(vel);

                // Start the recoil coroutine
                StartCoroutine(arm.Recoil());
            }
        }
    }

    private IEnumerator WaitToShoot()
    {
        yield return new WaitForSeconds(1);
        waitingToShoot = false;
    }
}
