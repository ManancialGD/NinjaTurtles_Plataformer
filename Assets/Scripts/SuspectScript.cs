using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SuspectScript : MonoBehaviour
{
    // (MIN  01   02   MAX)
    float suspectScale = 0f; // (0f - 3f - 6f - 10f)
    float updateCooldown = 0f;
    SpriteRenderer enemySpriteRenderer;
    SpriteRenderer suspectSprite;
    GameObject suspectObj;
    NativeInfo native;
    float enemyViewDistance = 10f;
    EnemyHP enemyHP;
    public Vector2 lastRockDetectedPosition;

    void Start()
    {
        enemySpriteRenderer = GetComponent<SpriteRenderer>();
        enemyHP = GetComponent<EnemyHP>();
        native = FindObjectOfType<NativeInfo>();

        if (GetComponent<Enemy2>() == null && GetComponent<Enemy_Ranger>() == null)
        {
            Debug.Log("ERROR 419624 - Script not assigned to an Enemy.");
            return;
        }

        suspectObj = new GameObject("SuspectSign");
        suspectObj.transform.SetParent(transform);
        suspectSprite = suspectObj.AddComponent<SpriteRenderer>();
        suspectSprite.sprite = native.BallSprite;
        suspectObj.transform.position = new Vector2(transform.position.x, transform.position.y + 0.5f);
        suspectObj.transform.localScale = new Vector2(0.3f, 0.3f);
    }

    // Update is called once per frame
    void Update()
    {
        // Update updateCooldown timer
        if (updateCooldown > 0f) updateCooldown -= Time.deltaTime;
        if (updateCooldown < 0f) updateCooldown = 0f;

        // Update suspectScale timer
        if (updateCooldown <= 0f)
        {
            float distance = 0f;
            if (suspectScale >= 6f)
            {
                if (distance > enemyViewDistance * 2 && suspectScale > 0f) suspectScale -= Time.deltaTime / 2; // -0.5/sec
            }
            else if (distance > enemyViewDistance && suspectScale > 0f) suspectScale -= Time.deltaTime / 2; // -0.5/sec

            if (suspectScale < 0f) suspectScale = 0f;
        }

        if (enemyHP.GetEnemyUnconsciousCooldown() > 0f)
        {
            suspectSprite.color = Color.white;
        }
        else if (suspectScale >= 3f)
        {
            if (suspectScale >= 6f) suspectSprite.color = Color.red; // Target player
            else suspectSprite.color = Color.yellow; // suspect
        }
        else
        {
            suspectSprite.color = Color.clear;
        }
    }

    public void Suspect(float distance, float maxDistance, Vector2 collisionPosition)
    {
        updateCooldown = 2f;
        if (distance <= maxDistance)
        {
            suspectScale += Mathf.Abs(distance - maxDistance);
            if (suspectScale > 10f) suspectScale = 10f;
        }
        lastRockDetectedPosition = collisionPosition;
        Debug.Log("suspectScale = " + suspectScale);
        return;
    }

    public float GetSuspectScale() => suspectScale;

}
