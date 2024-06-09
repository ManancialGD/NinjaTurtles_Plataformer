
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
public class DisplayGunAmmo : MonoBehaviour
{
    // 10 - 12.5
    Canvas canvas;
    Image image;
    Light2D lightComponent;
    GameObject thisEnemy;
    EnemyShooter enemyShooter;
    Vector3 defaultScale;
    Vector3 defaultPosition;
    void Start()
    {
        thisEnemy = GetComponentInParent<EnemyShooter>().gameObject;
        enemyShooter = thisEnemy.GetComponent<EnemyShooter>();
        lightComponent = GetComponentInChildren<Light2D>();
        canvas = GetComponentInChildren<Canvas>();
        image = canvas.GetComponentInChildren<Image>();
        defaultScale = transform.localScale;
        defaultPosition = transform.localPosition;
        Debug.Log("ThisEnemy: " + thisEnemy.name);
    }

    void Update()
    {
        if (thisEnemy == null) { Debug.LogError("Enemy not found!"); return; }
        if (enemyShooter == null) { Debug.LogError("Enemy script not found!"); return; }

        (float ammo, float maxValue) = enemyShooter.GetTimeUntilShoot();

        image.rectTransform.sizeDelta = new Vector2((maxValue - ammo) / maxValue * 5, image.rectTransform.sizeDelta.y);
        lightComponent.intensity = Random.Range((maxValue - ammo) / maxValue * 0.85f, (maxValue - ammo) / maxValue);

        if (Random.Range(0, 10) == 0)
        {
            lightComponent.intensity -= Random.Range(0.05f, 0.8f);
        }
    }
}
