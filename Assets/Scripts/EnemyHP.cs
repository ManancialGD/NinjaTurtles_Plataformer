using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    // Initiators
    Rigidbody2D rb;


    public float enemyUnconsciousCooldown = 0f;
    GameObject enemyUnconsciousSign;
    public Sprite BallSprite;

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
    Transform enemyTransform;

    private void Start()
    {
        enemyTransform = GetComponent<Transform>();
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
        if (enemyUnconsciousCooldown > 0f)
        {

            DrawCircle(enemyTransform.position.x, enemyTransform.position.y + 0.5f);
            enemyUnconsciousCooldown -= Time.deltaTime;



        }
        else
        {
            if (enemyUnconsciousCooldown < 0f) enemyUnconsciousCooldown = 0f;
            if (enemyUnconsciousSign != null) DeleteCircle(enemyUnconsciousSign);
        }





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
                TakeDamage(damageAmount, new Vector2(playerScript.playerAttackForce.x * newDir, playerScript.playerAttackForce.y), 2f);
                playerScript.BoostPlayer(new Vector2(playerScript.playerAttackForce.x * 0.1f * newDir, playerScript.playerAttackForce.y * 0.1f));
            }
        }
    }

    public void TakeDamage(int damage, Vector2 attackForce, float unconsciousTime)
    {

        if (unconsciousTime > 0f) enemyUnconsciousCooldown = unconsciousTime;

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

    void DrawCircle(float x, float y)
    {
        if (enemyUnconsciousSign != null) return;
        enemyUnconsciousSign = new GameObject("EnemyUnconsciousSign");
        enemyUnconsciousSign.transform.SetParent(GetComponent<Transform>());
        SpriteRenderer spriteRenderer = enemyUnconsciousSign.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = BallSprite;
        enemyUnconsciousSign.transform.position = new Vector2(x, y);
        enemyUnconsciousSign.transform.localScale = new Vector2(0.3f, 0.3f);
    }
    void DeleteCircle(GameObject obj)
    {
        Destroy(obj);
        enemyUnconsciousSign = null;
    }

    public float GetEnemyUnconsciousCooldown() {
        return enemyUnconsciousCooldown;
    }
}
