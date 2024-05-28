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
    LayerMask layersContact;


    void Start()
    {
        attackCooldown = Random.Range(attackTime[0], attackTime[1]);
        shootScript = GetComponentInChildren<EnemyAim>();
        flipped = false;
        native = FindObjectOfType<NativeInfo>();
        suspectScript = GetComponent<SuspectScript>();
        layersContact = (1 << LayerMask.NameToLayer("jumpableGround")) | (1 << LayerMask.NameToLayer("Player"));
        seeingPlayer_History = true;
    }


    void Update()
    {
        seeingPlayer = false;
        float magnitude_distance = 0f;
        Debug.log("AAA");
        Debug.Log("GetSuspectScale: " + suspectScript.GetSuspectScale());
        if (suspectScript.GetSuspectScale() >= 6)
        {
            Vector2 playerPos = native.GetSelectedPlayerPosition();
            (Vector2 updatedDistance, float magnitude) = native.GetDistance(transform.position, playerPos);
            magnitude_distance = magnitude;
            seeingPlayer = native.MakeLinecast(transform.position + new Vector3(0f, 0.25f, 0f), updatedDistance / magnitude, magnitude, layersContact).rigidbody.gameObject.GetComponent<PlayerScript>();
            Debug.log("seeingPlayer: " + seeingPlayer);
        }

        if (attackCooldown > 0f) attackCooldown -= Time.deltaTime;
        else if (attackCooldown <= 0f)
        {
            if (suspectScript.GetSuspectScale() >= 6)
            {
                if (seeingPlayer)
                {
                    Debug.log("attackCooldown: " + attackCooldown);
                    if (!seeingPlayer_History)
                    {
                        attackCooldown = Random.Range(0.4f, 1.5f);
                    }
                    else
                    {
                        if (magnitude_distance <= attackRange) // Attack player
                        {
                            //Debug.Log("Ranger - Shoot Player");
                            shootScript.ShootBullet();
                            attackCooldown = Random.Range(attackTime[0], attackTime[1]);
                        }
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
