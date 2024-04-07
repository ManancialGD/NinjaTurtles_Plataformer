using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    Player playerScript; // Reference to the Player script
    public float rotationSpeed = 5f;
    public float rotationModifier;
    public float moveSpeed = 5f;
    public float shootDamage = 40f;
    NativeInfo native;
    Rigidbody2D rb;

    CameraFollow cameraFollow;
    ExplosionOnDestroy explosionParticles;

    private void Start()
    {
        native = FindObjectOfType<NativeInfo>();
        explosionParticles = GetComponent<ExplosionOnDestroy>();
        cameraFollow = FindAnyObjectByType<CameraFollow>();
        playerScript = native.GetPlayerObj(native.currentPlayerID).GetComponent<Player>();

        rb = GetComponent<Rigidbody2D>();

        RotateTowardsPlayer();

        if (playerScript != null) ShootTowardsPlayer();

    }


    void RotateTowardsPlayer()
    {
        Vector3 vectorToTarget = playerScript.transform.position - transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - rotationModifier;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = q;
    }


    void ShootTowardsPlayer()
    {

        (Vector2, float) distance = native.GetDistance(transform.position, playerScript.transform.position);

        Vector2 normalizedDistance = distance.Item1 / distance.Item2;

        float angle = Mathf.Atan2(normalizedDistance.y, normalizedDistance.x) * Mathf.Rad2Deg;

        if (angle < 90 || angle > 270) angle += distance.Item2 * 3f;
        else angle -= distance.Item2 * 3f;

        if (angle > 360) angle -= 360;
        if (angle < 0) angle = 360 - Mathf.Abs(angle);

        angle = angle * Mathf.Deg2Rad;

        rb.velocity = new Vector3(Mathf.Cos(angle) * (moveSpeed + (distance.Item2 * (distance.Item2 * 0.015f))), Mathf.Sin(angle) * (moveSpeed + (distance.Item2 * (distance.Item2 * 0.015f))), 0f);
    }


    void OnTriggerEnter2D(Collider2D other)
    {

        if (!other.CompareTag("Enemy"))
        {
            if (other.CompareTag("Player")) cameraFollow.DamageCameraShake();
            explosionParticles.Explode(rb.velocity);
            Destroy(gameObject);
        }
        // Destroi o objeto ao entrar em colisão com outro objeto

    }
}
