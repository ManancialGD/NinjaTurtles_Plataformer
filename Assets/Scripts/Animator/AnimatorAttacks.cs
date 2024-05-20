using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Attacks : StateMachineBehaviour
{
    private LeoAttacks leoAttacks;
    private Player player;
    private int sampleRate = 10; // Animation sample rate (frames per second)
    [SerializeField] private int attackType;
    private AttackCollisionActivator attackCollisionActivator;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // initiate scripts
        player = FindObjectOfType<Player>();
        leoAttacks = FindObjectOfType<LeoAttacks>();

        // setup veriables
        leoAttacks.isAttacking = true;
        player.canMove = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attackCollisionActivator = FindObjectOfType<AttackCollisionActivator>();

        float animationLength = stateInfo.length; // Duration of the animation in seconds
        float frameDuration = 1f / sampleRate; // Duration of a single frame in seconds

        float currentTime = stateInfo.normalizedTime * animationLength; // Current time in the animation
        int currentFrame = Mathf.FloorToInt(currentTime / frameDuration); // Current frame index


        // Actions based on specific frames

        if (currentFrame == 2) // At frame 2
        {
            attackCollisionActivator.animationAttackType = attackType; // Will activate the collider, sending the code to AttaclCollisionActivator.cs, so it knows which collider to activate.
        }
        
        if (currentFrame == 3) // At frame 3
        {
            attackCollisionActivator.animationAttackType = 0; // This will make AttackCollisionActivator.cs desactivate the collider.
        }

        if (currentFrame == 6) // At frame 6
        {
            leoAttacks.isAttacking = false; // attacking is false if it reach the end of the animation
        }
        
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Setting up variables
        leoAttacks.isAttacking = false;
        if (leoAttacks.hasHit) leoAttacks.hasHit = false;
        if (!player.canMove) player.canMove = true;
    }


    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
