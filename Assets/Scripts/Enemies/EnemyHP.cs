using System.Collections;
using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    Rigidbody2D rb;
    EnemyAudio enemyAudio;

    [Header("Health")]
    
    [SerializeField] private int hp = 100;
    public int HP { get { return hp; } set { hp = value; if (hp <= 0) Die(); else if (hp >= maxHealth) hp = maxHealth; } }
    [SerializeField] private readonly int maxHealth = 100;
    [SerializeField] private bool hasInfHP;

    [Space]

    [Header("Stun")]
    [SerializeField] private float stunTime;
    public bool IsStunned { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyAudio = GetComponentInChildren<EnemyAudio>();

        IsStunned = false;
    }

    public virtual void TakeDamage(int damageAmount, float stunTime = 0, Vector2? knockback = null)
    {
        if (hasInfHP) damageAmount = 0; // if has damage amount, will damage 0 

        enemyAudio.PlaySwordHitSound();
        
        HP -= damageAmount;
 
        if (stunTime >= 0) Stun(stunTime);

        Vector2 appliedKnockback = knockback ?? new Vector2(0, 0);
        Knockback(appliedKnockback);
    }

    public virtual void Knockback(Vector2 knockback)
    {
        Vector3 velocity = rb.velocity;

        velocity.x += knockback.x;
        velocity.y += knockback.y;

        rb.velocity = velocity;
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    public void Stun(float stunTime)
    {
        StopAllCoroutines();
        IsStunned = true;
        StartCoroutine(StunTime(stunTime));
    }

    private IEnumerator StunTime(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        IsStunned = false;
    }
}
