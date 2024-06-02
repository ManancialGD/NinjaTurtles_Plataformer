using System.Collections;
using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    Rigidbody2D rb;

    [Header("Health")]
    
    [SerializeField] private int hp = 100;
    public int HP { get { return hp; } set { hp = value; if (hp <= 0) Die(); else if (hp >= maxHealth) hp = maxHealth; } }
    [SerializeField] private readonly int maxHealth;
    [SerializeField] private bool hasInfHP;

    [Space]

    [Header("Stun")]
    
    [SerializeField] private bool isStunned;
    [SerializeField] private float stunTime;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        isStunned = false;
    }

    public virtual void TakeDamage(int damageAmount, float stunTime = 0, Vector2? knockback = null)
    {
        if (hasInfHP) damageAmount = 0; // if has damage amount, will damage 0 

        HP -= damageAmount;
 
        if (stunTime <= 0) Stun(stunTime);

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
        isStunned = true;
        StartCoroutine(StunTime(stunTime));
    }

    private IEnumerator StunTime(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        isStunned = false;
    }
}
