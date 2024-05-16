using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeoAttacks : MonoBehaviour
{
    Player player;
    PlayerHP hp;
    Rigidbody2D rb;
    Collision coll;
    LeoAnimation anim;
    
    [SerializeField] private Vector2 slashAttackVelocity = new Vector2();
    [SerializeField] private Vector2 slashAttackVelocityLeft = new Vector2();
    [SerializeField] private Vector2 stabVelocity = new Vector2();
    [SerializeField] private Vector2 stabVelocityLeft = new Vector2();
    [SerializeField] private Vector2 groundSlamVelocity = new Vector2();
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collision>();
        anim = GetComponent<LeoAnimation>();
        hp = GetComponent<PlayerHP>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Attack") && ( Input.GetAxis("Horizontal") > .01f || Input.GetAxis("Horizontal") < -.01f ) && coll.onGround && !player.isPlayerAttacking && hp.playerStamina >= 25f)
        {
            anim.ChangeAnimation("LeoStab");
            hp.ConsumeStamina(25);
            if (anim.isFacingRight && !anim.isAttacking) rb.velocity += stabVelocity;
            else if (!anim.isAttacking) rb.velocity += stabVelocityLeft;
        }
        if (Input.GetButtonDown("Attack") && coll.onGround && !player.isPlayerAttacking && hp.playerStamina >= 25f)
        {
            anim.ChangeAnimation("LeoSlashAttack");
            hp.ConsumeStamina(25);
            if (anim.isFacingRight && !anim.isAttacking) rb.velocity += slashAttackVelocity;
            else if (!anim.isAttacking) rb.velocity += slashAttackVelocityLeft;
        }
        /*else if (Input.GetButtonDown("Attack") && Input.GetAxis("Vertical") < 0 && !coll.isNearGround)
        {
            anim.ChangeAnimation("LeoSlashAttack");
            if (!anim.isAttacking && !coll.isNearGround) rb.velocity += groundSlamVelocity;
        }*/ // THIS IS FOR THE GROUND SLAM, NOT WORKING CUZ DON'T HAVE THE ANIMATIONS FOR IT YET
    }
}
