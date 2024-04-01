using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ThrowRockScript : MonoBehaviour
{
    public Transform aimLayer;
    public GameObject cameraObj;
    private float ThrowRockCooldown = 0f;
    private Vector2 mousePosition_History = new Vector2(0f, 0f);
    private bool aiming = false;
    NativeInfo native;
    private float gravityScale = 0.5f;

    void Start()
    {
        native = FindObjectOfType<NativeInfo>();
    }
    void Update()
    {

        if (ThrowRockCooldown > 0f) ThrowRockCooldown -= Time.deltaTime;
        if (ThrowRockCooldown < 0f) ThrowRockCooldown = 0f;

        if (aiming) AimThrowRock();

    }

    public float GetThrowRockCooldown() => ThrowRockCooldown;

    public void SetThrowRockCooldown(float newTime) => ThrowRockCooldown = newTime;

    private void AimThrowRock()
    {

        bool updateGraphic = false;


        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);


        if (mousePos != mousePosition_History) updateGraphic = true;

        mousePosition_History = mousePos;

        if (!updateGraphic) return;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector2 playerPos = rb.position;

        //Vector2 cameraDifference = new Vector2(playerPos.x - cameraObj.transform.position.x, playerPos.y - cameraObj.transform.position.y);

        Vector2 distance = mousePos - playerPos;

        float inclination = Mathf.Atan2(distance.y, distance.x);
        float angularInclination = inclination * Mathf.Rad2Deg;


        if (angularInclination < 0) angularInclination = 360f - Mathf.Abs(angularInclination);

/*
        Vector2 negatives = new Vector2(1, 1);
        if (angularInclination > 90 && angularInclination < 180) negatives = new Vector2(-1, 1);
        else if (angularInclination > 180 && angularInclination < 270) negatives = new Vector2(-1, -1);
        else if (angularInclination > 270 && angularInclination < 360) negatives = new Vector2(1, -1);
*/

        Vector2 maxDistance = new Vector2(3f * Mathf.Cos(angularInclination * Mathf.Deg2Rad), 3f * Mathf.Sin(angularInclination * Mathf.Deg2Rad));
        Debug.Log(maxDistance);

        if (distance.x > 0 && distance.x > maxDistance.x) distance = new Vector2(maxDistance.x, distance.y);
        else if (distance.x < 0 && distance.x < maxDistance.x) distance = new Vector2(maxDistance.x, distance.y);

        if (distance.y > 0 && distance.y > maxDistance.y) distance = new Vector2(distance.x, maxDistance.y);
        else if (distance.y < 0 && distance.y < maxDistance.y) distance = new Vector2(distance.x, maxDistance.y);

        float magnitude = (Mathf.Abs(distance.x) + Mathf.Abs(distance.y)) * 3;



        Vector2 aimPos = playerPos;
        Vector2 velocity = new Vector2(distance.x * 0.8f , distance.y * 0.8f);

        foreach (Transform child in aimLayer)
        {
            Destroy(child.gameObject);
        }

        Debug.Log("AimThrowRock()");
        for (int i = 0; i < 10; i++)
        {
            GameObject aim = new GameObject("aim_" + i);

            aim.transform.SetParent(aimLayer);
            SpriteRenderer spriteRenderer = aim.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = native.BallSprite;
            spriteRenderer.color = Color.white;
            aim.transform.localScale = new Vector3(0.3f - (0.3f * i / 10), 0.3f - (0.3f * i / 10));

            aimPos += velocity / 3;
            velocity = new Vector2(velocity.x, velocity.y - gravityScale / 3);

            aim.transform.position = aimPos;

        }


    }

    public bool IsAiming() => aiming;
    public void StartAiming() => aiming = true;
    public void StopAiming()
    {
        aiming = false;
        foreach (Transform child in aimLayer)
        {
            Destroy(child.gameObject);
        }
        return;
    }

}
