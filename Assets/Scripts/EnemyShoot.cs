using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    public float speed = 50.0f;

    Rigidbody2D rb;
    PlayerHP playerHP;
    SpriteRenderer sp;
    ParticleSystem particleS;
    ExplosionOnDestroy explosionParticles;
    private bool alreadyAttacked;

    void Start()
    {
        explosionParticles = GetComponent<ExplosionOnDestroy>();
        alreadyAttacked = false;
        particleS = GetComponentInChildren<ParticleSystem>();
        sp = GetComponent<SpriteRenderer>();
        playerHP = FindObjectOfType<PlayerHP>();
        Destroy(gameObject, 5.0f);
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
        var em = particleS.emission;
        em.enabled = false;

        if (!alreadyAttacked)
        {
            if (other.GetComponent<Player>() != null)playerHP.DamagePlayer(25);
            explosionParticles.Explode(rb.velocity);
        }
        alreadyAttacked = true;



        Destroy(gameObject, 5f);
    }
}
