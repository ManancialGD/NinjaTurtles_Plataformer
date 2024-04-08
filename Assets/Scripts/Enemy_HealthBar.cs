using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Enemy_HealthBar : MonoBehaviour
{

    SpriteRenderer healthBar;
    EnemyHP enemyHealthScript;
    Vector2 MAX_SIZE;
    float health_history = 0;
    [SerializeField] float MaxHealth;

    void Start()
    {
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        enemyHealthScript = GetComponentInParent<EnemyHP>();

        healthBar = spriteRenderers[1];

        MAX_SIZE = healthBar.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        float enemyHealth = enemyHealthScript.health;
        if (enemyHealth != health_history)
        {
            healthBar.transform.localScale = new Vector2(ConvertLimit(enemyHealth, MaxHealth), healthBar.transform.localScale.y);

            if (enemyHealth != health_history)
            {
                if (enemyHealth < 100 / 10 * 3) healthBar.color = Color.red;
                else if (enemyHealth < 100 / 10 * 7) healthBar.color = Color.yellow;
                else healthBar.color = Color.green;
            }

            health_history = enemyHealth;

        }

    }
    float ConvertLimit(float value, float MaxValue) // 0 - 100
    {
        float result = value / MaxValue * MAX_SIZE.x;
        return result;
    }
}