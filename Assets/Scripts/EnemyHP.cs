using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    // Initiators
    Rigidbody2D rb;
    [Header("Floating Text")]
    public GameObject FloattingTextPrefab;

    [Space]

    [Header("Stats")]
    public int health = 30;

    [Header("Bools")]
    public bool hasInfinitHealth;
    public bool canTakeDamage;
    private int damageAmount;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Player playerScript = FindObjectOfType<Player>();

        if (playerScript != null)
        {
            damageAmount = playerScript.GetDamage();
        }
        else
        {
            Debug.LogError("Script do Player não encontrado.");
        }
    }

    private void Update() {
        canTakeDamage = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("AttackArea") && canTakeDamage)
        {
            canTakeDamage = false;
            PolygonCollider2D attackingArea = other.GetComponent<PolygonCollider2D>();

            if (attackingArea != null)
            {
                TakeDamage(damageAmount);
            }
        }
    }

    void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0 && !hasInfinitHealth)
        {
            Die();
        }

        if (FloattingTextPrefab) ShowFloatingText();
    }

    void ShowFloatingText()
    {
        Vector3 textPosition = new Vector3(transform.position.x, transform.position.y, -5f);
        var go = Instantiate(FloattingTextPrefab, textPosition, Quaternion.identity, transform);
        go.GetComponent<TextMesh>().text = damageAmount.ToString();
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
