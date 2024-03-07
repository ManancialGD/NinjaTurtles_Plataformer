using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Initiators
    GameObject player;
    Rigidbody2D rb;
    [Header("Floating Text")]
    public GameObject FloattingTextPrefab;
   
    [Space]

    [Header("Stats")]
    public int health = 30;

    private int damageAmount;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");

        if (player != null)
        {
            Player playerScript = player.GetComponent<Player>();

            if (playerScript != null)
            {
                damageAmount = playerScript.GetDamage();
            }
            else
            {
                Debug.LogError("Script do Player não encontrado no GameObject 'Player'.");
            }
        }
        else
        {
            Debug.LogError("GameObject 'Player' não encontrado.");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("AttackArea"))
        {
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

        if (health <= 0)
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
