using System;
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
    [SerializeField] private LayerMask enemyLayer;

    public bool isAttackingLeft = false;
    public bool wasAttackingLeft = false;
    public float combatModeArea;


    GameObject[] todosObjetos;
    GameObject closestEnemy;

    bool playerInCombatMode = false;

    void Start()
    {
        coll = GetComponent<Collision>();
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        todosObjetos = GameObject.FindObjectsOfType<GameObject>();

        float minEnemyDistance = combatModeArea;
        int enemysCount = 0;

        // Loop através de todos os objetos encontrados
        foreach (GameObject obj in todosObjetos)
        {
            if(obj.name == "Enemy") Debug.Log("Enemy exists");
            if (obj.layer == enemyLayer)
            {

                Transform objectTransform = obj.GetComponent<Transform>();
                float distanceLast = (rb.position.x - objectTransform.position.x) + (rb.position.y - objectTransform.position.y);

                if (Mathf.Abs(distanceLast) <= combatModeArea && Mathf.Abs(distanceLast) < minEnemyDistance) // Combat Mode
                {
                    enemysCount++;
                    closestEnemy = obj;
                    minEnemyDistance = Mathf.Abs(distanceLast);
                }

            }
        }

        // caso o player saia da Combat Zone

        if (enemysCount > 0)
        {
            Transform closestEnemyTransform = closestEnemy.GetComponent<Transform>();
            Debug.Log(closestEnemyTransform.name);

            float distance = (rb.position.x - closestEnemyTransform.position.x) + (rb.position.y - closestEnemyTransform.position.y);
            float distanceX = rb.position.x - closestEnemyTransform.position.x;
            Debug.Log("distanceX = " + distanceX);
            if (Mathf.Abs(distance) <= combatModeArea)
            {
                playerInCombatMode = true;
                if (distanceX > 0) sprite.flipX = false;
                else if (distanceX < 0) sprite.flipX = true;
            }
            else
            {
                playerInCombatMode = false;
            }
        }

        UpdateAnimationState();
    }
    private void LateUpdate()
    {
        if (!playerInCombatMode)
        {
            if (rb.velocity.x < 0) sprite.flipX = true;
            else if (rb.velocity.x > 0) sprite.flipX = false;
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

        }
        else if (rb.velocity.y < -0.1f && coll.onGround)
        {
            state = MovementState.Falling;
            anim.SetBool(holdOnWallParameter, false);
        }
        else if (rb.velocity.x >= 0.1f || rb.velocity.x <= -0.1f)
        {
            state = MovementState.Walk;
            anim.SetBool(holdOnWallParameter, false);
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
