using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompleteLevel : MonoBehaviour
{
    BoxCollider2D collider;
    [SerializeField] GameObject[] enemies;
    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "CompleteLevel")
        {
            foreach (GameObject enemy in enemies)
            {
                if (enemy != null) return;
            }
            FinishLevel(); // caso todos os inimigos estejam mortos
        }
    }

    private void FinishLevel()
    {
        Debug.Log("Você terminou o nível!");
    }
}
