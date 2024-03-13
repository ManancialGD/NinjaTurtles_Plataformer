using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    Collision coll;
    Animator anim;
    SpriteRenderer sprite;
    Rigidbody2D rb;
    private enum MovementState { Idle, Walk, Jump, Falling, OnWall, Attacking, LeftAttacking };
    private string holdOnWallParameter = "HoldOnWall";
    public bool canFlip = true;
    
    public bool isAttackingLeft = false;
    public bool wasAttackingLeft = false;

    void Start()
    {
        coll = GetComponent<Collision>();
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        UpdateAnimationState();
    }
    private void LateUpdate() {
        if (isAttackingLeft){
            sprite.flipX = false;
        }
        else if (!isAttackingLeft && wasAttackingLeft){
            sprite.flipX = true;
        }

    }
    private void UpdateAnimationState()
    {
        MovementState state;

        if (coll.onWall && !coll.onGround)
        {
            state = MovementState.OnWall;
            anim.SetBool(holdOnWallParameter, true);
            sprite.flipX = coll.onRightWall;
        }
        else if (Input.GetButtonDown("Attack") && coll.onGround && sprite.flipX == false)
        {
            state = MovementState.Attacking;
            anim.SetBool(holdOnWallParameter, false);
        }
        else if (Input.GetButtonDown("Attack") && coll.onGround && sprite.flipX == true)
        {
            state = MovementState.LeftAttacking;
            anim.SetBool(holdOnWallParameter, false);
        }
        else if (rb.velocity.y > 0.1f || rb.velocity.y < 0.1f && !coll.onGround)
        {
            state = MovementState.Jump;
            anim.SetBool(holdOnWallParameter, false);

            if (canFlip && !isAttackingLeft) sprite.flipX = rb.velocity.x < 0f;
        }
        else if (rb.velocity.y < -0.1f && coll.onGround)
        {
            state = MovementState.Falling;
            anim.SetBool(holdOnWallParameter, false);
            if (canFlip && !isAttackingLeft) sprite.flipX = rb.velocity.x < 0f;
        }
        else if (rb.velocity.x != 0f)
        {
            state = MovementState.Walk;
            anim.SetBool(holdOnWallParameter, false);
            if (canFlip && !isAttackingLeft) sprite.flipX = rb.velocity.x < 0f;
        }
        else
        {
            state = MovementState.Idle;
            anim.SetBool(holdOnWallParameter, false);
        }

        //Debug.Log(state);
        anim.SetInteger("state", (int)state);
    }
}
