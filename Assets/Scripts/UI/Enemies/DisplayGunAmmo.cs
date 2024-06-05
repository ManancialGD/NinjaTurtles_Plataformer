using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DisplayGunAmmo : MonoBehaviour
{
    // 10 - 12.5
    SpriteRenderer sr;
    GameObject thisEnemy;
    EnemyShooter enemyShooter;
    Vector3 defaultScale;
    Vector3 defaultPosition;
    void Start()
    {
        thisEnemy = GetComponentInParent<EnemyShooter>().gameObject;
        enemyShooter = thisEnemy.GetComponent<EnemyShooter>();
        sr = GetComponent<SpriteRenderer>();
        defaultScale = transform.localScale;
        defaultPosition = transform.localPosition;
        Debug.Log("ThisEnemy: " + thisEnemy.name);
    }

    void Update()
    {
        if (thisEnemy == null) { Debug.LogError("Enemy not found!"); return; }
        if (enemyShooter == null) { Debug.LogError("Enemy script not found!"); return; }

        float ammo = enemyShooter.GetEnemyAmmo();

        transform.localScale = new Vector3(ammo * defaultScale[0], transform.localScale.y, transform.localScale.z);
        transform.localPosition = new Vector3(defaultPosition[0] - ((1 - ammo) * 2.5f), defaultPosition.y, defaultPosition.z);
    }
}
