using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class HealthBarScript : MonoBehaviour
{

    SpriteRenderer healthBar;
    SpriteRenderer staminaBar;
    PlayerHP playerHealthScript;
    NativeInfo native;
    Vector2 MAX_SIZE;
    float health_history = 0;
    float stamina_history = 0;

    void Start()
    {
        native = FindObjectOfType<NativeInfo>();
        playerHealthScript = native.GetPlayerObj(native.currentPlayerID).GetComponent<PlayerHP>();

        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        healthBar = spriteRenderers[1];
        staminaBar = spriteRenderers[2];

        MAX_SIZE = healthBar.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerHealthScript.playerHealth != health_history)
        {
            healthBar.transform.localScale = new Vector2(ConvertLimit(playerHealthScript.playerHealth), healthBar.transform.localScale.y);

            if (playerHealthScript.playerHealth != health_history)
            {
                if (playerHealthScript.playerHealth < 100 / 10 * 3) healthBar.color = Color.red;
                else if (playerHealthScript.playerHealth < 100 / 10 * 7) healthBar.color = Color.yellow;
                else healthBar.color = Color.green;
            }

            health_history = playerHealthScript.playerHealth;

        }

        if (playerHealthScript.playerStamina != stamina_history)
        {
            staminaBar.transform.localScale = new Vector2(ConvertLimit(playerHealthScript.playerStamina), staminaBar.transform.localScale.y);
            stamina_history = playerHealthScript.playerStamina;
        }

    }
    float ConvertLimit(float value) // 0 - 100
    {
        float result = value / 100 * MAX_SIZE.x;
        return result;
    }
}