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
    LeoStats leoStats;
    MenuManager menuManager;
    LeoAudio leoAudio;

    private Vector2 input;

    [SerializeField] private Vector2 slashAttackVelocity = new Vector2();
    [SerializeField] private Vector2 slashAttackVelocityLeft = new Vector2();
    [SerializeField] private Vector2 stabVelocity = new Vector2();
    [SerializeField] private Vector2 stabVelocityLeft = new Vector2();
    [SerializeField] private Vector2 upperCutVelocity = new Vector2();
    [SerializeField] private Vector2 upperCutVelocityLeft = new Vector2();

    public bool isAttacking;
    public bool changedAttacks;
    public bool hasHit = false;

    void Awake()
    {
        inputs = GetComponent<PlayerInputs>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<LeoCollisionDetector>();
        leoAnim = GetComponent<LeoAnimation>();
        leoMov = GetComponent<LeoMovement>();
        leoStats = GetComponent<LeoStats>();
        menuManager = FindObjectOfType<MenuManager>();
        leoAudio = FindObjectOfType<LeoAudio>();
        // Insure that hasHit starts as false
        hasHit = false;
    }


    void Update()
    {
        if (menuManager.GamePaused) return;

        if (Input.GetButtonDown("Attack"))
        {
            if (coll.onGround)
            {
                if (leoStats.Stamina > 0)
                {
                    if (!leoStats.InStaminaBreak) CheckAttackType();
                }
            }
        }
    }

    /// <summary>
    /// Check Inputs for playing the right attack based on the input of the player
    /// </summary>
    private void CheckAttackType()
    {
        input = inputs.input; // Taking the input from the PlayerInput.cs to this class

        if (!isAttacking || hasHit) // If is already attacking, can't attack again. BUT only if the attack hit.
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
        }
    }


    void ExecuteAttack(string animationName, Vector2 velocityRight, Vector2 velocityLeft)
    {
        string currentAttack = "";
        if (currentAttack == animationName)
        {
            changedAttacks = false;
        }
        else changedAttacks = true;

        currentAttack = animationName;
        
        if (changedAttacks)
        {
            leoAnim.ChangeAnimation(animationName);
            isAttacking = true;
            leoStats.ConsumeStamina(20);
            if (leoMov.IsFacingRight && !isAttacking) rb.velocity += velocityRight;
            else if (!isAttacking) rb.velocity += velocityLeft;
        }
        leoAudio.PlayAttackSound();
    }
}
