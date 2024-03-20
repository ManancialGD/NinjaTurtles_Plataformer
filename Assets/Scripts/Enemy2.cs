using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2 : MonoBehaviour
{
    int enemyFixed;
    public float airAttackCooldown;
    Vector2 enemyFixedPosition;
    float airAttacking = 0f;
    bool onGround;
    public LayerMask playerLayer;
    static float groundFriction = 0.99f;
    PlayerHP playerHPScript;
    Player playerScript;
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
    public float climpForce;
    bool climbing;
    [SerializeField] private LayerMask jumpableGround;

    EnemyHP enemyHP;

    private enum MovementState { Idle, Walk, Jump, Falling, OnWall, Attacking, LeftAttacking };

    void Start()
    {
        enemyFixed = 0;
        climbing = false;
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        coll = GetComponents<BoxCollider2D>();

        enemyHP = GetComponent<EnemyHP>();
        playerHPScript = FindObjectOfType<PlayerHP>();
        playerScript = FindObjectOfType<Player>();

        OnGroundCollision();
    }

    void Update()
    {
        OnGroundCollision();

        if (enemyFixed == 2)
        {
            rb.position = enemyFixedPosition;
            rb.velocity = new Vector2(0f, 0f);
            return;
        }

        FollowPlayer();
        AttackPlayer_Event();
        GroundFriction();
    }

    private void FollowPlayer()
    {
        //Debug.Log("ENEMY UN - " + enemyHP.GetEnemyUnconsciousCooldown());
        if (enemyHP.GetEnemyUnconsciousCooldown() > 0f) return;
        else
        {

            playerRB = playerScript.GetComponent<Rigidbody2D>();

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

            if (Physics2D.BoxCast(coll[0].bounds.center, coll[0].bounds.size, 0f, Vector2.down, 0.0f, jumpableGround) && enemyFixed == 0|| Physics2D.BoxCast(coll[1].bounds.center, coll[1].bounds.size, 0f, Vector2.down, 0.0f, jumpableGround) && enemyFixed == 0)
            {
                climbing = true;

                int side_wall = 1;
                if (Physics2D.BoxCast(coll[0].bounds.center, coll[0].bounds.size, 0f, Vector2.down, 0.0f, jumpableGround)) side_wall = -1;

                if (distanceX > 0 && side_wall < 0 || distanceX < 0 && side_wall > 0)
                {
                    if (enemyFixed == 0 && airAttacking == 0f)
                    {
                        Brute_AirAttack(7f, 15f);
                        return;
                    }

                }

                rb.velocity = new Vector2(distanceX / Mathf.Abs(distanceX) * EnemySpeed * 0f, climpForce); // caso a formula seja necessária futuramente
            }
            else
            {
                climbing = false;
            }
        }
    }


    public void Brute_AirAttack(float JumpForce, float Damage)
    {
        Debug.Log("Brute Air Attack");

        airAttacking = Damage;
        enemyFixed = 2;
        enemyFixedPosition = rb.position;

        StartCoroutine(OnAirAttack(JumpForce, 0.7f));
        return;
    }
    private int AttackPlayer_Event()
    {
        if (enemyHP.GetEnemyUnconsciousCooldown() > 0f) return 0;
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

    void GroundFriction()
    {
        if (onGround) rb.velocity = new Vector2(rb.velocity.x * groundFriction, rb.velocity.y);
        return;
    }

    void OnGroundCollision()
    {
        if (Physics2D.OverlapBox(coll[2].bounds.center, coll[2].bounds.size, 0f, jumpableGround))
        {
            if (!onGround) onGround = true;
        }
        else
        {
            if (onGround) onGround = false;
            airAttacking = 0f;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && airAttacking > 0f)
        {

            playerHPScript.DamagePlayer(airAttacking);
            playerScript.BoostPlayer(new Vector2(rb.velocity.x * 0.5f, rb.velocity.x * 0.5f));
            airAttacking = 0f;

        }
    }

    private IEnumerator OnAirAttack(float JumpForce, float delayTime)
    {

        yield return new WaitForSeconds(delayTime);

        playerRB = playerScript.GetComponent<Rigidbody2D>();

        Vector2 playerPos = playerRB.position;
        Vector2 distance = playerPos - rb.position;
        Debug.Log(distance.x + ", " + distance.y);
        enemyFixed = 1;
        rb.velocity = new Vector2(distance.x * JumpForce * 0.66f, distance.y * JumpForce * 0.33f);
        
        yield return new WaitForSeconds(airAttackCooldown);
        enemyFixed = 0;
    }


}
