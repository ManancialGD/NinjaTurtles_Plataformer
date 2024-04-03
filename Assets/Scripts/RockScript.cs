using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RockScript : MonoBehaviour
{
    CircleCollider2D c_collider;
    SpriteRenderer spriteRenderer;
    NativeInfo native;
    float timeAlive = 0f;
    float MAX_TIME_ALIVE = 4f; // seconds

    bool collided = false;

    float suspectDistance = 7f;

    public
    void Start()
    {
        c_collider = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        native = FindObjectOfType<NativeInfo>();
    }


    void Update()
    {
        if (timeAlive < MAX_TIME_ALIVE)
        {
            timeAlive += Time.deltaTime;
            spriteRenderer.color = new Color(255f, 255f, 255f, (MAX_TIME_ALIVE - timeAlive) / MAX_TIME_ALIVE);
        }

        if (timeAlive >= MAX_TIME_ALIVE) Destroy(gameObject);

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        List<(GameObject, float)> enemiesDistances = native.GetEnemiesDistances(transform.position);

        for (int i = 0; i < enemiesDistances.Count; i++)
        {
            GameObject enemy = enemiesDistances[i].Item1;
            float distance = Mathf.Abs(enemiesDistances[i].Item2);

            int enemyType = 0; //NONE
            if (enemy.GetComponent<Enemy2>() != null) enemyType = 1; //BRUTE
            else if (enemy.GetComponent<Enemy_Ranger>() != null) enemyType = 2; //RANGER

            if (distance < suspectDistance)
            {
                if(collided && collision.gameObject.tag == "Enemy") return;
                if (enemyType == 1)
                {
                    SuspectScript suspectScript = enemy.GetComponent<SuspectScript>();
                    if (collided) suspectScript.Suspect(distance * 10f, suspectDistance, transform.position);
                    else suspectScript.Suspect(distance, suspectDistance, transform.position);
                }
                else if (enemyType == 2)
                {
                    SuspectScript suspectScript = GetComponent<SuspectScript>();
                    if (collided) suspectScript.Suspect(distance * 10f, suspectDistance, transform.position);
                    suspectScript.Suspect(distance, suspectDistance, transform.position);
                }
                else
                {
                    Debug.Log("ERROR 074124 - Invalid EnemyType.");
                }
            }
        }
        collided = true;
        return;
    }
}
