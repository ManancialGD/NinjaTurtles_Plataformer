using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages Leo's health, stamina, and status effects.
/// </summary>
public class LeoStats : MonoBehaviour
{
    Rigidbody2D rb;
    LeoMovement leoMov;
    LeoAttacks attacks;

    private bool wasAttacking;

    [Header("Health")]
    [SerializeField] private Image hpImage; // Image component to display health bar
    [SerializeField] private int hp = 100; // Current health points
    public int HP { get { return hp; } private set { hp = value; if (hp <= 0) Die(); else if (hp >= maxHealth) hp = maxHealth; } } // Property to access and modify HP
    [SerializeField] private int maxHealth = 100; // Maximum health points
    [SerializeField] private bool hasInfHP; // Infinite health flag

    [Space]

    [Header("Stamina")]
    [SerializeField] private Image staminaImage; // Image component to display stamina bar
    [SerializeField] private int stamina = 100; // Current stamina points
    public int Stamina { get { return stamina; } private set { stamina = value; if (stamina <= 0) StaminaBreak(); else if (stamina >= maxStamina) stamina = maxStamina; } } // Property to access and modify Stamina
    [SerializeField] private int maxStamina = 100; // Maximum stamina points
    [SerializeField] private bool hasInfStamina = false; // Infinite stamina flag
    [SerializeField] public bool InStaminaBreak { get; private set; } // Flag indicating if Leo is in a stamina break state
    [SerializeField] private GameObject staminaBreakPrefab; // Prefab for visualizing stamina break
    [SerializeField] private Vector3 spawnPrefabOffset; // Offset for spawning stamina break prefab

    [Space]

    [Header("Stun")]
    [SerializeField] private bool isStunned; // Flag indicating if Leo is currently stunned
    [SerializeField] private float stunTime; // Duration of stun

    private Coroutine passiveStaminaCoroutine; // Coroutine reference for passive stamina recovery

    /// <summary>
    /// Initializes necessary components and sets initial states.
    /// </summary>
    private void Awake()
    {
        leoMov = GetComponent<LeoMovement>();
        attacks = GetComponent<LeoAttacks>();
        rb = GetComponent<Rigidbody2D>();

        isStunned = false;
        InStaminaBreak = false;
    }

    /// <summary>
    /// Handles passive stamina recovery and attack-related stamina management.
    /// </summary>
    private void FixedUpdate()
    {
        if (attacks.isAttacking)
        {
            if (passiveStaminaCoroutine != null)
            {
                StopCoroutine(passiveStaminaCoroutine);
                passiveStaminaCoroutine = null;
            }
            wasAttacking = true;
        }

        if (Stamina < maxStamina)
        {
            if (wasAttacking && !attacks.isAttacking)
            {
                if (passiveStaminaCoroutine == null)
                {
                    passiveStaminaCoroutine = StartCoroutine(PassiveStaminaCoroutine());
                }
            }
            else if (!wasAttacking && !attacks.isAttacking)
            {
                ReceiveStamina(1);
            }
        }
    }

    // Method to heal Leo
    /// <summary>
    /// Increases Leo's health by the specified amount.
    /// </summary>
    public void Heal(int healAmount)
    {
        HP += healAmount;
    }

    // Method to apply damage to Leo
    /// <summary>
    /// Reduces Leo's health by the specified amount and applies additional effects like stun and knockback.
    /// </summary>
    public void TakeDamage(int damageAmount, float stunTime = 0, Vector2? knockback = null)
    {
        if (hasInfHP) damageAmount = 0; // If infinite health, no damage is taken 

        HP -= damageAmount;

        if (stunTime > 0) Stun(stunTime);

        Vector2 appliedKnockback = knockback ?? new Vector2(0, 0);
        Knockback(appliedKnockback);
    }

    // Method called when Leo's HP reaches zero
    /// <summary>
    /// Destroys Leo when his health reaches zero.
    /// </summary>
    private void Die()
    {
        Destroy(gameObject);
    }

    // Coroutine for passive stamina recovery
    private IEnumerator PassiveStaminaCoroutine()
    {
        yield return new WaitForSeconds(.5f);
        wasAttacking = false;
        if (Stamina < maxStamina)
        {
            ReceiveStamina(1);
        }
    }

    // Method to consume stamina
    /// <summary>
    /// Decreases Leo's stamina by the specified amount.
    /// </summary>
    public void ConsumeStamina(int staminaAmount)
    {
        if (hasInfStamina) staminaAmount = 0;

        Stamina -= staminaAmount;
        staminaImage.fillAmount = Stamina / 100f;
    }

    // Method to receive stamina
    /// <summary>
    /// Increases Leo's stamina by the specified amount and handles stamina break recovery.
    /// </summary>
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

    // Coroutine for managing stamina break
    private IEnumerator StaminaBreakCoroutine()
    {
        while (attacks.isAttacking)
        {
            yield return null; // Wait for the next frame until the attack is finished
        }

        stamina = 0; // Set stamina to 0 only after the attack is finished

        InstantiateStaminaBreakPrefab();

        leoMov.StaminaBroke();
    }

    // Method to trigger stamina break
    private void StaminaBreak()
    {
        InStaminaBreak = true;
        StartCoroutine(StaminaBreakCoroutine());
    }

    // Method to instantiate stamina break prefab
    public void InstantiateStaminaBreakPrefab()
    {
        Vector3 spawnPosition = transform.position + spawnPrefabOffset;
        Instantiate(staminaBreakPrefab, spawnPosition, Quaternion.identity);
    }

    // Method to apply knockback to Leo
    /// <summary>
    /// Applies knockback force to Leo.
    /// </summary>
    public virtual void Knockback(Vector2 knockback)
    {
        Vector3 velocity = rb.velocity;

        velocity.x += knockback.x;
        velocity.y += knockback.y;

        rb.velocity = velocity;
    }

    // Method to stun Leo
    /// <summary>
    /// Stuns Leo for the specified duration.
    /// </summary>
    public void Stun(float stunTime)
    {
        isStunned = true;
        StartCoroutine(StunTime(stunTime));
    }

    // Coroutine for managing stun duration
    private IEnumerator StunTime(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        isStunned = false;
    }
}