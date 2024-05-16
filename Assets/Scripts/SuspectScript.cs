using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SuspectScript : MonoBehaviour
{
    // (MIN  01   02   MAX)
    [SerializeField]
    Vector2 initialPosition;
    private LayerMask groundLayer;

    [SerializeField] LayerMask choosenLayers;

    float suspectScale = 0f; // (0f - 3f - 6f - 10f)
    float updateCooldown = 0f;
    SpriteRenderer enemySpriteRenderer;
    SpriteRenderer suspectSprite;
    GameObject suspectObj;
    NativeInfo native;
    float enemyViewDistance = 300f;
    EnemyHP enemyHP;
    public Vector2 lastRockDetectedPosition;

    float playerLastViewCooldown = 0f;
    float playerViewTime = 0f;
    bool viewedPlayer = false;

    void Start()
    {

        if (transform == null) return;

        initialPosition = transform.position;
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

        if (viewedPlayer) playerLastViewCooldown += Time.deltaTime;

        if (suspectScale >= 6)
        {
            viewedPlayer = true;
            playerLastViewCooldown = 0f;
        }

        if (playerViewTime > 1.5f)
        {
            suspectScale = 10f;
        }

        //Debug.Log("suspectScale = " + suspectScale);

        // Update updateCooldown timer
        if (updateCooldown > 0f) updateCooldown -= Time.deltaTime;
        if (updateCooldown < 0f)
        {
            updateCooldown = 0f;
            lastRockDetectedPosition = initialPosition;
        }

        // Update suspectScale timer
        if (updateCooldown <= 0f)
        {

            if (suspectScale > 0f) suspectScale -= Time.deltaTime / 2; // -0.5/sec
            if (suspectScale < 0f) suspectScale = 0f;

            if (suspectScale < 3f)
            {
                lastRockDetectedPosition = initialPosition;
            }

        }

        if (enemyHP.GetEnemyUnconsciousCooldown() > 0f)
        {
            suspectSprite.color = Color.white;
        }
        else if (suspectScale >= 3f)
        {
            if (suspectScale >= 6f) suspectSprite.color = Color.red; // Target player
            else suspectSprite.color = Color.yellow; // Suspect
        }
        else
        {
            suspectSprite.color = Color.clear;
        }

        //float[] rayAngles = { 180f / 6f * 5f - 90f, 180f / 6f * 4f - 90f, 180f / 6f * 3f - 90f, 180f / 6f * 2f - 90f, 180f / 6f * 1f - 90f };
        //float[] rayAngles = new float[2];

        int layerMaskValue = LayerMask.GetMask("Player", "jumpableGround",  "jumpableWalls"); 

        View(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + 0.25f), layerMaskValue); // Enemy head

        if (playerLastViewCooldown > 3f)
        {
            playerLastViewCooldown = 0f;
            viewedPlayer = false;
            suspectScale = 2.5f;
            updateCooldown = 2f;

            lastRockDetectedPosition = initialPosition;
        }
    }

    public void Suspect(float distance, float maxDistance, Vector2 collisionPosition)
    {
        updateCooldown = 2f;
        if (distance <= maxDistance)
        {
            suspectScale += Mathf.Abs(distance - maxDistance);
            if (suspectScale > maxDistance) suspectScale = maxDistance;
        }
        lastRockDetectedPosition = collisionPosition;
        return;
    }

    public float GetSuspectScale() => suspectScale;
    public float SetSuspectScale(float value) => suspectScale = value;

    private bool View(Vector2 position, int contactLayers)
    {

        bool playerViewed = false;

        float minAngle = 324;
        float maxAngle = 36;

        if (enemySpriteRenderer.flipX)
        {
            minAngle = 144;
            maxAngle = 216;
        }

        if (minAngle < 0) minAngle = 360 - Mathf.Abs(minAngle);
        if (maxAngle < 0) maxAngle = 360 - Mathf.Abs(maxAngle);

        //Debug.Log("MinAngle = " + minAngle + " | MaxAngle = " + maxAngle);

        Vector2 playerPos = native.GetSelectedPlayerPosition() + new Vector2(0f, 0.25f);
        Vector2 distance = playerPos - position;
        float radAngle = Mathf.Atan2(distance.y, distance.x);
        float currentAngle = radAngle * Mathf.Rad2Deg;
        if (currentAngle < 0) currentAngle = 360 - Mathf.Abs(currentAngle);

        Color rayColor = Color.white;

        float magnitude = native.GetDistance(playerPos, position).Item2;
        Vector2 direction = new Vector2(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad));

        RaycastHit2D hit = Physics2D.Linecast(position, position + (direction * enemyViewDistance), contactLayers);
        //RaycastHit2D hit = Physics2D.Raycast(position, direction, contactLayers, 1, 10f);

        bool seeingPlayer = false;
        if (hit.rigidbody != null && hit.rigidbody.gameObject != null) Debug.Log("Hit: " + hit.rigidbody.gameObject.name);
        if (hit.rigidbody != null && hit.rigidbody.gameObject != null && hit.rigidbody.gameObject.CompareTag("Player"))
        {
            playerViewed = true;
            playerLastViewCooldown = 0f;
            rayColor = Color.red;
            seeingPlayer = true;
        }
        else
        {
            playerViewTime = 0f;
            if (suspectScale >= 6f) suspectScale -= Time.deltaTime / 10;
        }

        if (!enemySpriteRenderer.flipX || gameObject.transform.localScale.x < 0) // Right
        {
            if (currentAngle < minAngle && currentAngle > maxAngle)
            {
                rayColor = Color.yellow;
                seeingPlayer = false;
            }
        }
        else// Left
        {
            if (currentAngle < minAngle || currentAngle > maxAngle)
            {
                rayColor = Color.yellow;
                seeingPlayer = false;
            }
        }

        if (seeingPlayer)
        {
            float addedValue = Mathf.Abs(Mathf.Cos(radAngle)) + (1 - Mathf.Cos(36 * Mathf.Deg2Rad));

            addedValue += (addedValue - 1) * 7.5f;

            playerViewTime += addedValue * Time.deltaTime; // min: 1 (6 secs) | max: 1.7865 ( 0-6 suspectScale em 6 / 3.358 sec )
        }
        Debug.DrawRay(position, direction * enemyViewDistance, rayColor);

        return playerViewed;
    }

}
