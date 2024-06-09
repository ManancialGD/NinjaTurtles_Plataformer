using System.Collections;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    DetectLeo leodetection;
    ShooterAim aim;
    RotateArm arm;
    Transform target;
    EnemyHP hp;
    EnemyAudio enemyAudio;

    [Header("Times")]
    private (float, float) cooldownTime = (1.8f, 2.5f);
    private (float, float) timeUntilShoot = (2f, 2f); // Item1 - Valor atualizado  |  Item2 - Tempo que foi setado

    [Space]

    [Header("Bools")]
    private bool firstDetected;

    private void Awake()
    {
        hp = GetComponent<EnemyHP>();
        leodetection = GetComponent<DetectLeo>();
        enemyAudio = GetComponentInChildren<EnemyAudio>();
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
                    timeUntilShoot.Item1 = Random.Range(cooldownTime.Item1, cooldownTime.Item2);
                    timeUntilShoot.Item2 = timeUntilShoot.Item1;
                    firstDetected = false;
                }
                // Start shooting if not already waiting to shoot
                if (timeUntilShoot.Item1 <= 0 && !hp.IsStunned)
                {
                    StartCoroutine(ShootBullet(0f, vel));
                    timeUntilShoot.Item1 = Random.Range(cooldownTime.Item1, cooldownTime.Item2);
                    timeUntilShoot.Item2 = timeUntilShoot.Item1;
                }
            }
        }
        else
        {
            arm.SetRotationAngle(0);
            firstDetected = true;
        }
    }

    void Update()
    {
        if (timeUntilShoot.Item1 > 0) timeUntilShoot.Item1 -= Time.deltaTime;
    }

    private IEnumerator ShootBullet(float time, Vector2 vel)
    {
        yield return new WaitForSeconds(time);

        var newShot = Instantiate(aim.prefabToSpawn, aim.transform.position, Quaternion.identity);
        newShot.SetVelocity(vel);

        enemyAudio.PlayShootSound();

        StartCoroutine(arm.Recoil());
    }

    public (float, float) GetTimeUntilShoot() => (timeUntilShoot.Item1, timeUntilShoot.Item2);
}