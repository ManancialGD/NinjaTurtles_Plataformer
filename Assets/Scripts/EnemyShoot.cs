using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    Player playerScript; // Reference to the Player script
    PlayerHP playerHPScript;
    public float rotationSpeed = 5f;
    public float rotationModifier;
    public float moveSpeed = 5f;
    public float shootDamage = 40f;

    private void Start()
    {
        playerScript = FindObjectOfType<Player>();
        playerHPScript = FindObjectOfType<PlayerHP>();

        RotateTowardsPlayer();

    }

    private void Update()
    {
        if (playerScript != null)
        {
            MoveTowardsPlayer();
        }
    }

    void RotateTowardsPlayer()
    {
        Vector3 vectorToTarget = playerScript.transform.position - transform.position;
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
        Debug.Log(other.tag);
        if (!other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
        // Destroi o objeto ao entrar em colisão com outro objeto

    }
}
