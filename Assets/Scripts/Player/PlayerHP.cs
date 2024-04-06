using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    public Enemy2 enemy2;
    public EnemyShoot enemyShoot;
    public GameObject FloattingTextPrefab;

    [Header("Stats")]
    public float playerHealth = 100;
    public float playerStamina = 100;


    private float damageAmount;

    void Update()
    {
        if (playerHealth <= 0)
        {
            if (gameObject) PlayerDied();
            return;
        }
    }

    public int DamagePlayer(float damage)
    {
        playerHealth -= damage;

        if (playerHealth <= 0)
        {
            PlayerDied();
        }
        if (FloattingTextPrefab) ShowFloatingText();
        return 1;
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
    int PlayerDied()
    {
        if (gameObject)
        {
            Destroy(gameObject);
            return 1;
        }
        else
        {
            return 0;
        }
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
}
