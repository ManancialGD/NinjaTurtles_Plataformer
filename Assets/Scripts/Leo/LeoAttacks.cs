using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeoAttacks : MonoBehaviour
{
    Rigidbody2D rb;
    LeoCollisionDetector coll;
    LeoMovement leoMov;
    LeoAnimation leoAnim;
    PlayerInputs inputs;

    private Vector2 input;

    [SerializeField] private Vector2 slashAttackVelocity = new Vector2();
    [SerializeField] private Vector2 slashAttackVelocityLeft = new Vector2();
    [SerializeField] private Vector2 stabVelocity = new Vector2();
    [SerializeField] private Vector2 stabVelocityLeft = new Vector2();
    [SerializeField] private Vector2 upperCutVelocity = new Vector2();
    [SerializeField] private Vector2 upperCutVelocityLeft = new Vector2();
    [SerializeField] private Vector2 groundSlamVelocity = new Vector2();


    public bool isAttacking;
    public bool hasHit = false;

    void Awake()
    {
        inputs = GetComponent<PlayerInputs>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<LeoCollisionDetector>();
        leoAnim = GetComponent<LeoAnimation>();
        leoMov = GetComponent<LeoMovement>();
        //hp = GetComponent<PlayerHP>();

        // Insure that hasHit starts as false
        hasHit = false;
    }

    /// <summary>
    /// Check Inputs for playing the right attack animations and aplying the right velocities
    /// </summary>
    void Update()
    {
        input = inputs.input; // Taking the input from the PlayerInput.cs to this class

        if (Input.GetButtonDown("Attack") /*&& hp.playerStamina >= 25f*/) // Needs 25 Stamina to attack
        {
            if(!isAttacking || hasHit) // If is already attacking, can't attack again. BUT only if the attack hit.
            {    
                if (coll.onGround)
                {
                    if (input.y < -0.01f) // Check if player is pressing "S"
                    {
                        ExecuteAttack("LeoUpperCut", upperCutVelocity, upperCutVelocityLeft); // UpperCut Attack
                    }
                    else if (input.x > 0.01f || input.x < -0.01f) // Check if player is pressing "a" or "d"
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
        leoAnim.ChangeAnimation(animationName);
        /*hp.ConsumeStamina(25);*/
        if (leoMov.IsFacingRight && !isAttacking) rb.velocity += velocityRight;
        else if (!isAttacking) rb.velocity += velocityLeft;
    }
}
