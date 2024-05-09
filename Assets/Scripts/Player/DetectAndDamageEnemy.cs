using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectAndDamageEnemy : MonoBehaviour
{
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private Vector2 knockbackRight = new Vector2(200, 0);
    [SerializeField] private Vector2 knockbackLeft = new Vector2(-200, 0);
    [SerializeField] LeoAnimation leoAnimation;

    private void Start() {
        leoAnimation = FindObjectOfType<LeoAnimation>();
    }

    // This method is called when another collider enters this collider's trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the other collider has an EnemyHP component
        EnemyHP enemyHP = other.GetComponent<EnemyHP>();
        if (enemyHP != null)
        {
            // Check if leoAnimation is initialized
            if (leoAnimation != null)
            {
                if (leoAnimation.isFacingRight) enemyHP.TakeDamage(damageAmount, knockbackRight, 1f);
                else enemyHP.TakeDamage(damageAmount, knockbackLeft, 1f);
            }
            
        }
    }
}
