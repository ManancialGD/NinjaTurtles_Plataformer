using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    GameObject player;
    public float rotationSpeed = 5f;
    public float rotationModifier;
    public float moveSpeed = 5f;

    private void Start()
    {
        player = GameObject.Find("Player");

        if (player != null)
        {
            RotateTowardsPlayer();
        }
    }

    private void Update()
    {
        if (player != null)
        {
            MoveTowardsPlayer();
        }
    }

    void RotateTowardsPlayer()
    {
        Vector3 vectorToTarget = player.transform.position - transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - rotationModifier;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = q;
    }

    void MoveTowardsPlayer()
    {
        // Move o objeto para a frente (na direção do jogador)
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Destroi o objeto ao entrar em colisão com outro objeto
        Destroy(gameObject);
    }
}
