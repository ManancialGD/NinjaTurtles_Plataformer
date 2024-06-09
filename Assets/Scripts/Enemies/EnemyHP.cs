using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    private Rigidbody2D rb;
    private EnemyAudio enemyAudio;
    private List<SpriteRenderer> sp;

    [Header("Health")]
    [SerializeField] private int hp = 100;
    public int HP
    {
        get { return hp; }
        set
        {
            hp = value;
            if (hp <= 0)
            {
                Die();
            }
            else if (hp >= maxHealth)
            {
                hp = maxHealth;
            }
        }
    }
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private bool hasInfHP;

    [Header("Stun")]
    [SerializeField] private float stunTime;
    public bool IsStunned { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D not found on " + gameObject.name);
        }

        SpriteRenderer d_sp = GetComponent<SpriteRenderer>();
        SpriteRenderer[] detectSP_children = GetComponentsInChildren<SpriteRenderer>();

        sp = new List<SpriteRenderer>();
        if (d_sp != null)
        {
            sp.Add(d_sp);
        }

        foreach (SpriteRenderer s in detectSP_children)
        {
            if (s != null)
            {
                sp.Add(s);
            }
        }

        enemyAudio = GetComponentInChildren<EnemyAudio>();
        if (enemyAudio == null)
        {
            Debug.LogError("EnemyAudio not found in children of " + gameObject.name);
        }

        IsStunned = false;
    }

    public virtual void TakeDamage(int damageAmount, float stunTime = 0, Vector2? knockback = null)
    {
        if (hasInfHP)
        {
            damageAmount = 0; // if hasInfHP is true, damageAmount is set to 0
        }

        if (enemyAudio != null)
        {
            enemyAudio.PlaySwordHitSound();
        }

        HP -= damageAmount;

        if (stunTime > 0)
        {
            Stun(stunTime);
        }

        Vector2 appliedKnockback = knockback ?? Vector2.zero;
        Knockback(appliedKnockback);
        StartCoroutine(DamageColor(1f));
    }

    public virtual void Knockback(Vector2 knockback)
    {
        if (rb != null)
        {
            Vector3 velocity = rb.velocity;

            velocity.x += knockback.x;
            velocity.y += knockback.y;

            rb.velocity = velocity;
        }
        else
        {
            Debug.LogError("Rigidbody2D is null, cannot apply knockback");
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    public void Stun(float stunTime)
    {
        StopAllCoroutines();
        IsStunned = true;
        StartCoroutine(StunTimeCoroutine(stunTime));
    }

    private IEnumerator StunTimeCoroutine(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        IsStunned = false;
    }

    private IEnumerator DamageColor(float time)
    {
        foreach (SpriteRenderer s in sp)
        {
            if (s != null)
            {
                s.color = new Color(1, 0.5f, 0.6f, 1);
            }
        }
        yield return new WaitForSeconds(time);
        foreach (SpriteRenderer s in sp)
        {
            if (s != null)
            {
                s.color = Color.white;
            }
        }
    }
}