using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pizza : MonoBehaviour
{
    [SerializeField] private int healAmount = 20;

    private void OnTriggerEnter2D(Collider2D other)
    {
        LeoStats leo;
        leo = other.GetComponent<LeoStats>();
        if (leo != null)
        {
            leo.Heal(healAmount);
            Destroy(gameObject);
        }
    }
}
