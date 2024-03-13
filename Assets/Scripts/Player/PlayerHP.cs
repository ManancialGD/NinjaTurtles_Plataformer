using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    [Header("Stats")]
    public float PlayerHealth = 100;
    
    void Update()
    {
        if (PlayerHealth <= 0)
        {
            if (gameObject) PlayerDied();
            return;
        }
    }

    public int DamagePlayer(float damage)
    {
        PlayerHealth -= damage;

        if (PlayerHealth <= 0)
        {
            PlayerDied();
        }
        return 1;
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
}
