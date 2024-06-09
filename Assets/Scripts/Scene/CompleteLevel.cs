using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompleteLevel : MonoBehaviour
{
    SceneManage sceneManage;
    [SerializeField] GameObject[] enemies;

    private void Awake()
    {
        sceneManage = FindObjectOfType<SceneManage>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        LeoMovement leo = other.GetComponent<LeoMovement>();
        if (leo != null)
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
        sceneManage.ChangeScene("Win");
    }
}
