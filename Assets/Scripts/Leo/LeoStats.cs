using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LeoStats : MonoBehaviour
{
    // Initializers
    Rigidbody2D rb;
    LeoMovement leoMov;
    LeoAttacks attacks;

    [Header("Health")]
    [SerializeField] private Image hpImage;
    [SerializeField] private int hp = 100;
    public int HP { get { return hp; } private set { hp = value; if (hp <= 0) Die(); else if (hp >= maxHealth) hp = maxHealth; } }
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private bool hasInfHP;

    [Space]

    [Header("Stamina")]
    [SerializeField] private Image staminaImage;
    [SerializeField] private int stamina = 100;
    public int Stamina { get { return stamina; } private set { stamina = value; if (stamina <= 0) StaminaBreak(); else if (stamina >= maxStamina) stamina = maxStamina; } }
    [SerializeField] private int maxStamina = 100;
    [SerializeField] private bool hasInfStamina = false;
    [SerializeField] public bool InStaminaBreak { get; private set; }
    [SerializeField] private GameObject staminaBreakPrefab;
    [SerializeField] private Vector3 spawnPrefabOffset;
    [Space]

    [Header("Stun")]
    [SerializeField] private bool isStunned;
    [SerializeField] private float stunTime;

    private void Awake()
    {
        leoMov = GetComponent<LeoMovement>();
        attacks = GetComponent<LeoAttacks>();
        rb = GetComponent<Rigidbody2D>();

        isStunned = false;
        InStaminaBreak = false;
    }

    private void FixedUpdate()
    {
        if (Stamina < maxStamina)
        {
            if (!attacks.isAttacking) ReceiveStamina(1);
        }
    }

    // HP Logics
    public void Heal(int healAmount)
    {
        HP += healAmount;
    }

    public void TakeDamage(int damageAmount, float stunTime = 0, Vector2? knockback = null)
    {
        if (hasInfHP) damageAmount = 0; // if has damage amount, will damage 0 

        HP -= damageAmount;

        if (stunTime > 0) Stun(stunTime);

        Vector2 appliedKnockback = knockback ?? new Vector2(0, 0);
        Knockback(appliedKnockback);
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    // Stamina Logics

    public void ConsumeStamina(int staminaAmount)
    {
        if (hasInfStamina) staminaAmount = 0;

        Stamina -= staminaAmount;
        staminaImage.fillAmount = Stamina / 100f;
    }

    public void ReceiveStamina(int staminaAmount)
    {
        Stamina += staminaAmount;
        if (Stamina == maxStamina)
        {
            if (InStaminaBreak)
            {
                InStaminaBreak = false;
                leoMov.StaminaRecovered();
            }
        }
        staminaImage.fillAmount = Stamina / 100f;
    }

    private void StaminaBreak()
    {
        stamina = 0;
        InStaminaBreak = true;

        InstantiateStaminaBreakPrefab();

        leoMov.StaminaBroke();
    }

    public void InstantiateStaminaBreakPrefab()
    {

        Vector3 spawnPosition = transform.position + spawnPrefabOffset;
        Instantiate(staminaBreakPrefab, spawnPosition, Quaternion.identity);


    }

    // KnockBack and Stun
    public virtual void Knockback(Vector2 knockback)
    {
        Vector3 velocity = rb.velocity;

        velocity.x += knockback.x;
        velocity.y += knockback.y;

        rb.velocity = velocity;
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
