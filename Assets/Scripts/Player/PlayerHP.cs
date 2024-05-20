using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    public UnityEngine.UI.Image healthBar;
    public UnityEngine.UI.Image staminaBar;
    Player player;
    LeoAttacks leoAttacks;
    public Enemy2 enemy2;
    public EnemyShoot enemyShoot;
    public GameObject FloattingTextPrefab;

    [Header("Stats")]
    public float playerHealth;
    public float playerStamina;

    [Space]

    [Header("Bools")]
    [SerializeField] private bool infStamina = false;


    private float damageAmount;

    void Start()
    {
        leoAttacks = GetComponent<LeoAttacks>();
        player = GetComponent<Player>();
        playerHealth = 100f;
        healthBar.fillAmount = playerHealth / 100f;
        playerStamina = 100f;
    }
    void Update()
    {
        if (playerHealth <= 0)
        {
            if (gameObject) PlayerDied();
            return;
        }
        if (playerStamina < 100 && !leoAttacks.isAttacking)
        {
            playerStamina += Time.deltaTime * 25f; //  10 / sec  |  FULL / 10sec
            staminaBar.fillAmount = playerStamina/100f;
        }
        if (playerStamina > 100)
        {
            playerStamina = 100;
            staminaBar.fillAmount = 1;
        }
    }

    public void DamagePlayer(float damage)
    {
        var before = playerHealth;
        playerHealth -= damage;
        healthBar.fillAmount = playerHealth / 100f;

        Debug.Log($"Before={before}, after={playerHealth}");

        if (playerHealth <= 0f)
        {
            PlayerDied();
        }
        if (FloattingTextPrefab) ShowFloatingText();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("MeeleAttack"))
        {
            damageAmount = enemy2.damageAmount;
            DamagePlayer(damageAmount);

        }
        if (other.CompareTag("ShootAttack"))
        {
            damageAmount = enemyShoot.shootDamage;
            DamagePlayer(damageAmount);
        }
    }
    private void PlayerDied()
    {
        SceneManager.LoadScene("GameOver");
        Debug.Log("Player Died");      
    }
    void ShowFloatingText()
    {
        Vector3 textPosition = new Vector3(transform.position.x, transform.position.y, -5f);
        var go = Instantiate(FloattingTextPrefab, textPosition, Quaternion.identity, transform);
        go.GetComponent<TextMesh>().text = damageAmount.ToString();
        TextMesh textMesh = go.GetComponent<TextMesh>();
        textMesh.text = damageAmount.ToString();
        textMesh.color = Color.blue;
    }

    public void ConsumeStamina(float stamina)
    {
        if (infStamina) return;
        playerStamina -= stamina;
        staminaBar.fillAmount = stamina / 100f;
        if (playerStamina < 0) 
        {
            playerStamina = 0; 
            staminaBar.fillAmount = 0f;
        }

    }
}
