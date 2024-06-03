using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectAndDamageEnemy : MonoBehaviour
{
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private Vector2 knockbackRight = new Vector2(200, 0);
    [SerializeField] private Vector2 knockbackLeft = new Vector2(-200, 0);

    [SerializeField] private LeoMovement leoMov;
    [SerializeField] private LeoAttacks attacks;

    [SerializeField] private float circleRadius = 1f;
    [SerializeField] private LayerMask enemyLayer; // To filter out which objects to detect

    private void Awake()
    {
        leoMov = FindObjectOfType<LeoMovement>();
        attacks = FindObjectOfType<LeoAttacks>();
    }

    private void Update()
    {
        DetectEnemies();
    }

    private void DetectEnemies()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, circleRadius, enemyLayer);
        if (!attacks.hasHit)
        {
            foreach (Collider2D enemy in hitEnemies)
            {
                EnemyHP enemyHP = enemy.GetComponent<EnemyHP>();
                if (enemyHP != null)
                {
                    if (leoMov != null)
                    {
                        if (attacks.changedAttacks)
                        {
                            if (leoMov.IsFacingRight)
                                enemyHP.TakeDamage(damageAmount, 1f, knockbackRight);
                            else
                                enemyHP.TakeDamage(damageAmount, 1f, knockbackLeft);
                        }
                    }
                    attacks.hasHit = true;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, circleRadius);
    }
}
