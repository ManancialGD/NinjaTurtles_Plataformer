
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
    Player playerScript;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerScript = FindObjectOfType<Player>();

        if (playerScript != null)
        {
            damageAmount = playerScript.GetDamage();
        }
        else
        {
            Debug.LogError("Script do Player não encontrado.");
        }
    }

    private void Update()
    {
        canTakeDamage = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("AttackArea") && canTakeDamage)
        {
            canTakeDamage = false;
            PolygonCollider2D attackingArea = other.GetComponent<PolygonCollider2D>();

            Vector2 distance = new Vector2(rb.position.x - playerScript.GetPlayerPosition().x, rb.position.y - playerScript.GetPlayerPosition().y);

            if (attackingArea != null)
            {
                int newDir = -1;
                if (distance.x > 0) newDir = 1;
                TakeDamage(damageAmount, new Vector2(playerScript.playerAttackForce.x * newDir, playerScript.playerAttackForce.y));
                playerScript.BoostPlayer(new Vector2(playerScript.playerAttackForce.x * 0.1f * newDir, playerScript.playerAttackForce.y * 0.1f));
            }
        }
    }

    public void TakeDamage(int damage, Vector2 attackForce)
    {

        rb.velocity = new Vector2(rb.velocity.x + attackForce.x, rb.velocity.y + attackForce.y);

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
