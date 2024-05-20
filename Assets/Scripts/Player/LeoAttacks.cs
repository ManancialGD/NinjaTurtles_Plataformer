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
    [SerializeField] private Vector2 upperCutVelocity = new Vector2();
    [SerializeField] private Vector2 upperCutVelocityLeft = new Vector2();
    [SerializeField] private Vector2 groundSlamVelocity = new Vector2();


    public bool isAttacking;
    public bool hasHit = false;

    void Start()
    {
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collision>();
        anim = GetComponent<LeoAnimation>();
        hp = GetComponent<PlayerHP>();
    }

    /// <summary>
    /// Check Inputs for playing the right attack animations and aplying the right velocities
    /// </summary>
    void Update()
    {
        if (Input.GetButtonDown("Attack") && hp.playerStamina >= 25f) // Needs 25 Stamina to attack
        {
            if(!isAttacking || hasHit) // If is already attacking, can't attack again. BUT only if the attack hit.
            {    
                if (coll.onGround)
                {
                    if (Input.GetAxis("Vertical") < -.01f) // Check if player is pressing "S"
                    {
                        ExecuteAttack("LeoUpperCut", upperCutVelocity, upperCutVelocityLeft); // UpperCut Attack
                    }
                    else if ( Input.GetAxis("Horizontal") > .01f || Input.GetAxis("Horizontal") < -.01f ) // Check if player is pressing "a" or "d"
                    {
                        ExecuteAttack("LeoStab", stabVelocity, stabVelocityLeft); // Stab Attack
                    }
                    else // if is not pressing left/right nor down
                    {
                        ExecuteAttack("LeoSlashAttack", slashAttackVelocity, slashAttackVelocityLeft); // Slash Attack
                    }
                    /*else if (Input.GetButtonDown("Attack") && Input.GetAxis("Vertical") < 0 && !coll.isNearGround)
                    {
                        ExecuteAttack("LeoSlashAttack", groundSlamVelocity, Vector2.zero); // Groundslam
                    }*/
                }
            }
        }
    }


    void ExecuteAttack(string animationName, Vector2 velocityRight, Vector2 velocityLeft)
    {
        anim.ChangeAnimation(animationName);
        hp.ConsumeStamina(25);
        if (anim.isFacingRight && (!isAttacking)) rb.velocity += velocityRight;
        else if (!isAttacking) rb.velocity += velocityLeft;
    }
}
