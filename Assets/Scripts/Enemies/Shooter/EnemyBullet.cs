using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [Header("Speed")]
    public float speed = 50.0f;

    [Space]
    [Header("Destroy Time")]
    [SerializeField] private float destroyTime = 5f;

    [SerializeField] ParticleSystem particles;
    Rigidbody2D rb;
    LeoStats leoStats;
    SpriteRenderer sp;
    private bool alreadyAttacked;

    void Awake()
    {
        alreadyAttacked = false;
        sp = GetComponent<SpriteRenderer>();
        leoStats = FindObjectOfType<LeoStats>();
        rb = GetComponent<Rigidbody2D>(); // Ensure Rigidbody2D is initialized here
        Destroy(gameObject, destroyTime);
    }

    public void SetVelocity(Vector2 vec)
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        rb.velocity = vec;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        sp.color = new Color(0, 0, 0, 0);

        if (!alreadyAttacked)
        {
            if (other.GetComponent<LeoStats>() != null)
            {
                if (rb.velocity.x > 0.01f)
                {
                    leoStats.TakeDamage(25, 1, new Vector2 (150, 0));
                }
                else leoStats.TakeDamage(25, 1, new Vector2(-150, 0));
            }
                
            alreadyAttacked = true;
        }

        var emissionModule = particles.emission;
        emissionModule.enabled = false;
        Destroy(gameObject, 5f);
    }
}

