using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2 : MonoBehaviour
{
    public LayerMask playerLayer;
    PlayerHP playerHPScript;
    Rigidbody2D rb;
    Rigidbody2D playerRB;
    SpriteRenderer sprite;
    BoxCollider2D[] coll;
    Animator anim;
    public float EnemySpeed;
    public float EnemyAcceleration;
    public float DistanceArea;
    public float movementLerp = 4f;
    public int EnemyAttack;
    public bool inAttackZone;
    bool canAttack = true;
    bool isAttacking = false;
    public float damageAmount = 10;

    private enum MovementState { Idle, Walk, Jump, Falling, OnWall, Attacking, LeftAttacking };

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        coll = GetComponents<BoxCollider2D>();
        playerHPScript = FindObjectOfType<PlayerHP>();
    }

    void Update()
    {
        FollowPlayer();
        AttackPlayer_Event();
    }

    private void FollowPlayer()
    {
        playerRB = playerHPScript.GetComponent<Rigidbody2D>();

        float distanceX = playerRB.position.x - rb.position.x;
        float distanceY = playerRB.position.y - rb.position.y;

        if (Mathf.Abs(distanceX) > DistanceArea)
        {
            if (distanceX < 0)
            {
                if (rb.velocity.x - EnemyAcceleration <= -EnemySpeed) rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(-EnemySpeed, rb.velocity.y)), movementLerp * Time.deltaTime);
                else rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(rb.velocity.x - EnemyAcceleration, rb.velocity.y)), movementLerp * Time.deltaTime);
            }
            else
            {
                if (rb.velocity.x + EnemyAcceleration >= EnemySpeed) rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(EnemySpeed, rb.velocity.y)), movementLerp * Time.deltaTime);
                else rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(rb.velocity.x + EnemyAcceleration, rb.velocity.y)), movementLerp * Time.deltaTime);
            }
        }
    }

    private int AttackPlayer_Event()
    {
        inAttackZone = Physics2D.OverlapBox((Vector2)transform.position, new Vector2(1f, 0f), 0f, playerLayer);

        sprite.flipX = playerRB.position.x - rb.position.x < 0;

        if (inAttackZone && canAttack && !isAttacking)
        {
            MovementState state;
            state = MovementState.Attacking;
            anim.SetInteger("state", (int)state);
            MakeAttack();

            return 1;
        }
        else
        {
            MovementState state;
            if (Mathf.Abs(rb.velocity.x) > 0.1)
                state = MovementState.Walk;
            else
                state = MovementState.Idle;

            anim.SetInteger("state", (int)state);

            return 0;
        }
    }

    private int MakeAttack()
    {
        if (!canAttack) return 0;
        canAttack = false;
        isAttacking = true;

        StartCoroutine(PlayerCantAttack(0.4f));

        return 1;
    }

    private IEnumerator PlayerCantAttack(float delay)
    {
        yield return new WaitForSeconds(delay);

        MovementState state;
        if (Mathf.Abs(rb.velocity.x) > 0.1)
            state = MovementState.Walk;
        else
            state = MovementState.Idle;

        anim.SetInteger("state", (int)state);
        //Debug.Log("Terminado");
        isAttacking = false;
        yield return new WaitForSeconds(delay);
        canAttack = true;
    }
}
