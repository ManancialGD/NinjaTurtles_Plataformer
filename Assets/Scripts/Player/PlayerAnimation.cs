using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{

    public CameraFollow cameraFollow;
    public Player playerScript;
    Collision coll;
    Animator anim;
    SpriteRenderer sprite;
    Rigidbody2D rb;
    float playerAttackCooldown = 0f;
    private enum MovementState { Idle, Walk, Jump, Falling, OnWall, Attacking, LeftAttacking };
    private string holdOnWallParameter = "HoldOnWall";
    public bool canFlip = true;
    [SerializeField] private LayerMask enemyLayer;

    public bool isAttackingLeft = false;
    public bool wasAttackingLeft = false;
    public float combatModeArea;


    GameObject[] todosObjetos;
    GameObject closestEnemy;
    Vector2 dir;

    bool playerInCombatMode = false;

    void Start()
    {
        cameraFollow = FindObjectOfType<CameraFollow>();
        playerScript = FindObjectOfType<Player>();

        coll = GetComponent<Collision>();
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        dir = new Vector2(x, y);
        if (playerAttackCooldown > 0f) playerAttackCooldown -= Time.deltaTime;
        else if (playerAttackCooldown < 0f)
        {
            playerAttackCooldown = 0f;
            playerScript.DisablePlayerAttack();
        }

        todosObjetos = GameObject.FindObjectsOfType<GameObject>();

        float minEnemyDistance = combatModeArea;
        int enemysCount = 0;

        // Loop através de todos os objetos encontrados
        foreach (GameObject obj in todosObjetos)
        {
            if (obj.CompareTag("Enemy"))
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
            float distance = Mathf.Abs(rb.position.x - closestEnemyTransform.position.x) + Mathf.Abs(rb.position.y - closestEnemyTransform.position.y);
            float distanceX = rb.position.x - closestEnemyTransform.position.x;

            if (Mathf.Abs(distance) <= combatModeArea)
            {
                playerInCombatMode = true;
                if (distanceX > 0) sprite.flipX = true;
                else if (distanceX < 0) sprite.flipX = false;
                cameraFollow.CombatMode(closestEnemyTransform.position.x, closestEnemyTransform.position.y);
            }
            else
            {
                playerInCombatMode = false;
            }
        }
        else
        {
            playerInCombatMode = false;
        }

        UpdateAnimationState();
    }
    private void LateUpdate()
    {
        if (!playerInCombatMode)
        {
            if (rb.velocity.x < -0.1) sprite.flipX = true;
            else if (rb.velocity.x > 0.1) sprite.flipX = false;
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
            playerScript.EnablePlayerAttack();
        }
        else if (Input.GetButtonDown("Attack") && coll.onGround && sprite.flipX == true)
        {
            state = MovementState.LeftAttacking;
            anim.SetBool(holdOnWallParameter, false);
            playerScript.EnablePlayerAttack();

        }
        else if (Input.GetButtonDown("Attack") && dir.y < -0.1 && !coll.onGround && playerScript.GetPlayerDistanceFromGround() > 2.5f) //Air attack (down)
        {
            state = MovementState.Attacking;
            anim.SetBool(holdOnWallParameter, false);
            playerScript.EnablePlayerAttack();
            playerScript.AirAttack_Down();

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

    public void SetAttackCooldown(float timeout)
    {
        playerAttackCooldown = timeout;
        return;
    }

    public GameObject GetClosestEnemy(float detectionArea)
    {
        todosObjetos = GameObject.FindObjectsOfType<GameObject>();

        float minEnemyDistance = detectionArea;
        int enemysCount = 0;
        GameObject newClosestEnemy = null;

        // Loop através de todos os objetos encontrados
        foreach (GameObject obj in todosObjetos)
        {
            if (obj.CompareTag("Enemy"))
            {

                Transform objectTransform = obj.GetComponent<Transform>();
                float distanceLast = (rb.position.x - objectTransform.position.x) + (rb.position.y - objectTransform.position.y);

                if (Mathf.Abs(distanceLast) <= detectionArea && Mathf.Abs(distanceLast) < minEnemyDistance) // Combat Mode
                {
                    enemysCount++;
                    newClosestEnemy = obj;
                    minEnemyDistance = Mathf.Abs(distanceLast);
                }

            }
        }

        // caso o player saia da Combat Zone

        if (enemysCount > 0 && newClosestEnemy != null)
        {
            Transform closestEnemyTransform = newClosestEnemy.GetComponent<Transform>();
            float distance = Mathf.Abs(rb.position.x - closestEnemyTransform.position.x) + Mathf.Abs(rb.position.y - closestEnemyTransform.position.y);
            float distanceX = rb.position.x - closestEnemyTransform.position.x;

            if (Mathf.Abs(distance) <= detectionArea)
            {
                return newClosestEnemy;
            }
        }
        return null;
    }


}
