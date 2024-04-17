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
    private Vector2 mousePosition_History = Vector2.zero;
    private bool aiming = false;
    NativeInfo native;
    private float gravityScale = 4f;

    void Start()
    {
        native = FindObjectOfType<NativeInfo>();
    }
    void Update()
    {

        if (ThrowRockCooldown > 0f) ThrowRockCooldown -= Time.deltaTime;
        if (ThrowRockCooldown < 0f) ThrowRockCooldown = 0f;

        if (aiming)
        {

            AimThrowRock();

        }


    }

    public float GetThrowRockCooldown() => ThrowRockCooldown;

    public void SetThrowRockCooldown(float newTime) => ThrowRockCooldown = newTime;

    private void AimThrowRock()
    {

        bool updateGraphic = false;
        bool shoot = Input.GetMouseButton(0);

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (mousePos != mousePosition_History || mousePosition_History == Vector2.zero) updateGraphic = true;

        mousePosition_History = mousePos;

        if (!updateGraphic && !shoot) return;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector2 playerPos = new Vector2(rb.position.x, rb.position.y + 28/2);

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

        Vector2 maxDistance = new Vector2(100f * Mathf.Cos(angularInclination * Mathf.Deg2Rad), 100f * Mathf.Sin(angularInclination * Mathf.Deg2Rad));
        Debug.Log(maxDistance);

        if (distance.x > 0 && distance.x > maxDistance.x) distance = new Vector2(maxDistance.x, distance.y);
        else if (distance.x < 0 && distance.x < maxDistance.x) distance = new Vector2(maxDistance.x, distance.y);

        if (distance.y > 0 && distance.y > maxDistance.y) distance = new Vector2(distance.x, maxDistance.y);
        else if (distance.y < 0 && distance.y < maxDistance.y) distance = new Vector2(distance.x, maxDistance.y);

        float magnitude = (Mathf.Abs(distance.x) + Mathf.Abs(distance.y)) * 3;

        Vector2 aimPos = playerPos;
        Vector2 velocity = new Vector2(distance.x * 0.8f, distance.y * 0.8f);
        Vector2 initialVelocity = velocity * 4.75f;

        foreach (Transform child in aimLayer)
        {
            Destroy(child.gameObject);
        }

        Debug.Log("AimThrowRock()");

        int aim_dots = 20;
        float aim_dots_size = 15f;

        for (int i = 0; i < aim_dots; i++)
        {
            GameObject aim = new GameObject("aim_" + i);

            aim.transform.SetParent(aimLayer);
            SpriteRenderer spriteRenderer = aim.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = native.BallSprite;
            spriteRenderer.color = Color.white;
            Vector3 aim_scale = new Vector3(aim_dots_size - (aim_dots_size * i / aim_dots), aim_dots_size - (aim_dots_size * i / aim_dots));
            aim.transform.localScale = aim_scale;

            aimPos += velocity / 9;
            velocity = new Vector2(velocity.x, velocity.y - gravityScale);

            aim.transform.position = aimPos;

            /*
            RaycastHit2D raycast = Physics2D.CircleCast(aimPos, aim_scale.x / 2, Vector2.zero, 0f, native.jumpableGround_layer);
            Debug.Log("raycast = " + raycast.rigidbody);
            if (raycast.rigidbody)
            { //collision
                velocity = new Vector2(velocity.x, velocity.y * -0.8f);
            }
            */

        }

        if (shoot)
        {
            StopAiming();
            ThrowRock(playerPos, initialVelocity);
        }


    }

    private void ThrowRock(Vector2 pos, Vector2 vel)
    {
        GameObject aim = new GameObject("rock");

        //aim.transform.SetParent(aimLayer);
        SpriteRenderer spriteRenderer = aim.AddComponent<SpriteRenderer>();
        Rigidbody2D rb = aim.AddComponent<Rigidbody2D>();
        rb.mass = 15f;
        rb.gravityScale = 6f;
        CircleCollider2D collider = aim.AddComponent<CircleCollider2D>();
        CircleCollider2D colliderDetector = aim.AddComponent<CircleCollider2D>();
        aim.AddComponent<RockScript>();
        spriteRenderer.sprite = native.BallSprite;
        spriteRenderer.color = Color.white;
        aim.transform.localScale = new Vector3(9.6f, 9.6f);
        collider.radius = 0.1f;
        colliderDetector.radius = 0.15f;
        colliderDetector.isTrigger = true;
        collider.excludeLayers = native.GetPlayerLayerMask();
        collider.sharedMaterial = native.rockMaterial;
        rb.velocity = vel;
        aim.transform.position = pos;
    }
    public bool IsAiming() => aiming;
    public void StartAiming() => aiming = true;
    public void StopAiming()
    {
        aiming = false;
        mousePosition_History = Vector2.zero;
        foreach (Transform child in aimLayer)
        {
            Destroy(child.gameObject);
        }
        return;
    }

}
