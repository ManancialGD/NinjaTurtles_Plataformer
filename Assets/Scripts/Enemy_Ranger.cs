using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Ranger : MonoBehaviour
{
    // Start is called before the first frame update

    public bool flipped = false;
    public float attackCooldown;
    EnemyAim aimScript;
    EnemyShoot shootScript;
    public float attackRange;
    public float[] attackTime;
    NativeInfo native;
    SuspectScript suspectScript;
    Transform target;
    EnemyShoot bullet;

    bool seeingPlayer_History;
    bool seeingPlayer;
    [SerializeField] int layersContact;


    void Start()
    {   
        shootScript = FindAnyObjectByType<EnemyShoot>();
        target = FindObjectOfType<Player>().transform;
        attackCooldown = Random.Range(attackTime[0], attackTime[1]);
        aimScript = GetComponentInChildren<EnemyAim>();
        flipped = false;
        native = FindObjectOfType<NativeInfo>();
        suspectScript = GetComponent<SuspectScript>();
        seeingPlayer_History = true;
    }


    void Update()
    {
        if (gameObject.transform.position.x > target.transform.position.x)
        {
            Flip(true);
        }
        else Flip(false);

        seeingPlayer = false;
        float magnitude_distance = 0f;
        /*if (suspectScript.GetSuspectScale() >= 6)
        {
            Vector2 playerPos = native.GetSelectedPlayerPosition();
            (Vector2 updatedDistance, float magnitude) = native.GetDistance(transform.position, playerPos);
            magnitude_distance = magnitude;
            RaycastHit2D hit = native.MakeLinecast(transform.position + new Vector3(0f, 0.25f, 0f), updatedDistance / magnitude, 320, layersContact);
            if (hit.rigidbody != null && hit.rigidbody.gameObject != null) seeingPlayer = hit.rigidbody.gameObject.CompareTag("Player");
        }*/

        if (attackCooldown > 0f) attackCooldown -= Time.deltaTime;
        else if (attackCooldown <= 0f /*&& suspectScript.GetSuspectScale() >= 6*/)
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
                        if (aimScript.ComputeVelocity(transform.position, target.position, shootScript.speed, Physics2D.gravity.y, aimScript.minimizeTime, out Vector2 vel))
                        {
                            var newShot = Instantiate(bullet, transform.position, Quaternion.identity);
                            newShot.SetVelocity(vel);
                        }
                        else
                        {
                            Debug.LogWarning("Impossible to hit target!");
                        }
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
                transform.localScale = new Vector3(1, 1, 1);
                flipped = false;
            }
            else if (transform.position.x < playerObj.transform.position.x && !flipped)
            {
                transform.localScale = new Vector3(-1, 1, 1);
                flipped = true;
            }

        }
        seeingPlayer_History = seeingPlayer;
    }

    private void Flip(bool b)
    {
        if (b)
        {
            transform.localScale = new Vector3(-1, 1, 1); // Flip horizontally
            flipped = true;
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1); // Reset flip
            flipped = false;
        }
    }
}
