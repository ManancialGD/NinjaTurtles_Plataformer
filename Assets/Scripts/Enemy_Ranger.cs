using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Ranger : MonoBehaviour
{
    // Start is called before the first frame update

    Player playerScript;
    Transform thisTransform;
    public bool flipped = false;
    public float attackCooldown;
    EnemyAim shootScript;
    public float attackRange;
    public float[] attackTime;
    NativeInfo native;


    void Start()
    {
        attackCooldown = Random.Range(attackTime[0], attackTime[1]);
        shootScript = GetComponentInChildren<EnemyAim>();
        flipped = false;
        playerScript = FindObjectOfType<Player>();
        thisTransform = GetComponent<Transform>();
        native = FindObjectOfType<NativeInfo>();
    }

    // Update is called once per frame
    void Update()
    {

        if (attackCooldown > 0f) attackCooldown -= Time.deltaTime;
        else if (attackCooldown <= 0f)
        {
            Vector2 playerPos = native.GetSelectedPlayerPosition();
            Vector2 distance = new Vector2(transform.position.x - playerPos.x, transform.position.y - playerPos.y);

            if (Mathf.Abs(distance.x) + Mathf.Abs(distance.y) <= attackRange) // Attack player
            {
                //Debug.Log("Ranger - Shoot Player");
                shootScript.ShootBullet();
                attackCooldown = Random.Range(attackTime[0], attackTime[1]);
            }
        }


        if (transform.position.x < thisTransform.position.x)
        {
            thisTransform.localScale = new Vector3(1, thisTransform.localScale.y, thisTransform.localScale.z);
            flipped = false;
        }
        else
        {
            thisTransform.localScale = new Vector3(-1, thisTransform.localScale.y, thisTransform.localScale.z);
            flipped = true;
        }
    }

}
