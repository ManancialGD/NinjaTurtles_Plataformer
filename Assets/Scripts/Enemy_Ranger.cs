using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Ranger : MonoBehaviour
{
    // Start is called before the first frame update

    public bool flipped = false;
    public float attackCooldown;
    EnemyAim shootScript;
    public float attackRange;
    public float[] attackTime;
    NativeInfo native;
    SuspectScript suspectScript;

    bool seeingPlayer_History;
    bool seeingPlayer;
    [SerializeField] int layersContact;


    void Start()
    {
        attackCooldown = Random.Range(attackTime[0], attackTime[1]);
        shootScript = GetComponentInChildren<EnemyAim>();
        flipped = false;
        native = FindObjectOfType<NativeInfo>();
        suspectScript = GetComponent<SuspectScript>();
        seeingPlayer_History = true;
    }


    void Update()
    {
        seeingPlayer = false;
        float magnitude_distance = 0f;
        if (suspectScript.GetSuspectScale() >= 6)
        {
            Vector2 playerPos = native.GetSelectedPlayerPosition();
            (Vector2 updatedDistance, float magnitude) = native.GetDistance(transform.position, playerPos);
            magnitude_distance = magnitude;
            RaycastHit2D hit = native.MakeLinecast(transform.position + new Vector3(0f, 0.25f, 0f), updatedDistance / magnitude, 320, layersContact);
            if (hit.rigidbody != null && hit.rigidbody.gameObject != null) seeingPlayer = hit.rigidbody.gameObject.CompareTag("Player");
        }

        if (attackCooldown > 0f) attackCooldown -= Time.deltaTime;
        else if (attackCooldown <= 0f && suspectScript.GetSuspectScale() >= 6)
        {

            if (seeingPlayer)
            {

                if (!seeingPlayer_History)
                {
                    attackCooldown = Random.Range(0.4f, 1.5f);
                }
                else
                {
                    Debug.Log("Ranger - seeing (" + magnitude_distance + ")");
                    if (magnitude_distance <= attackRange) // Attack player
                    {
                        //Debug.Log("Ranger - Shoot Player");
                        shootScript.ShootBullet();
                        Debug.Log("Ranger - shoot");
                        attackCooldown = Random.Range(attackTime[0], attackTime[1]);
                    }
                }
            }
        }

        if (seeingPlayer)
        {

            GameObject playerObj = native.GetPlayerObj(native.currentPlayerID);

            if (transform.position.x > playerObj.transform.position.x && flipped)
            {
                transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
                flipped = false;
            }
            else if (transform.position.x < playerObj.transform.position.x && !flipped)
            {
                transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
                flipped = true;
            }

        }
        seeingPlayer_History = seeingPlayer;
    }

}
