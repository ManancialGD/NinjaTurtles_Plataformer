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
    static float groundFriction = 0.9f;
    [SerializeField] private float attackArea;
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
    SuspectScript suspectScript;
    Vector2 lastRockDetectedPosition;

    EnemyHP enemyHP;
    NativeInfo native;

    private enum MovementState { Idle, Walk, Jump, Falling, OnWall, Attacking, LeftAttacking };

    int playerID;

    void Start()
    {
        enemyFixed = 0;
        climbing = false;
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        coll = GetComponents<BoxCollider2D>();
        suspectScript = GetComponent<SuspectScript>();
        enemyHP = GetComponent<EnemyHP>();

        native = FindObjectOfType<NativeInfo>();
        playerID = native.currentPlayerID;
        playerHPScript = native.GetPlayerObj(native.currentPlayerID).GetComponent<PlayerHP>();
        playerScript = native.GetPlayerObj(native.currentPlayerID).GetComponent<Player>();

        OnGroundCollision();
    }

    void Update()
    {
        if (playerID != native.currentPlayerID)
        {
            playerHPScript = native.GetPlayerObj(native.currentPlayerID).GetComponent<PlayerHP>();
            playerScript = native.GetPlayerObj(native.currentPlayerID).GetComponent<Player>();
            playerID = native.currentPlayerID;
        }

        playerRB = native.GetPlayerObj(native.currentPlayerID).GetComponent<Rigidbody2D>();

        OnGroundCollision();

        if (enemyFixed == 2)
        {
            rb.position = enemyFixedPosition;
            rb.velocity = new Vector2(0f, 0f);
            return;
        }

        if (suspectScript.GetSuspectScale() > 6f)
        {
            FollowPlayer();
            AttackPlayer_Event();
        }


        GroundFriction();
    }

    private void FollowPlayer()
    {
        //Debug.Log("ENEMY UN - " + enemyHP.GetEnemyUnconsciousCooldown());
        float distanceX;
        float distanceY;
        bool freezeEnemy = false;
        if (native.GetDistance(transform.position, playerRB.position).Item2 < DistanceArea && suspectScript.GetSuspectScale() > 6f)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            freezeEnemy = true;
        }
        if (enemyHP.GetEnemyUnconsciousCooldown() > 0f) return;
        else if (suspectScript.GetSuspectScale() >= 3f && suspectScript.GetSuspectScale() < 6f)
        {

            distanceX = suspectScript.lastRockDetectedPosition.x - rb.position.x;
            distanceY = suspectScript.lastRockDetectedPosition.y - rb.position.y;

        }
        else if (suspectScript.GetSuspectScale() > 6f)
        {
            distanceX = playerRB.position.x - rb.position.x;
            distanceY = playerRB.position.y - rb.position.y;
        }
        else return;

        if (Mathf.Abs(distanceX) + Mathf.Abs(distanceY) > 0.4f) sprite.flipX = distanceX < 0; // para não alterar a direção quando pára

        if (Mathf.Abs(distanceX) > DistanceArea || suspectScript.GetSuspectScale() < 6f)
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

        if (Physics2D.BoxCast(coll[0].bounds.center, coll[0].bounds.size, 0f, Vector2.down, 0.0f, jumpableGround) && enemyFixed == 0 || Physics2D.BoxCast(coll[1].bounds.center, coll[1].bounds.size, 0f, Vector2.down, 0.0f, jumpableGround) && enemyFixed == 0)
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
            if (!freezeEnemy) rb.velocity = new Vector2(distanceX / Mathf.Abs(distanceX) * EnemySpeed * 0f, climpForce); // caso a formula seja necessária futuramente
        }
        else
        {
            climbing = false;
        }

    }


    public void Brute_AirAttack(float JumpForce, float Damage)
    {

        airAttacking = Damage;
        enemyFixed = 2;
        enemyFixedPosition = rb.position;

        StartCoroutine(OnAirAttack(JumpForce, 0.7f));
        return;
    }
    private int AttackPlayer_Event()
    {
        if (enemyHP.GetEnemyUnconsciousCooldown() > 0f) return 0;
        //inAttackZone = Physics2D.OverlapBox((Vector2)transform.position, new Vector2(1f, 0f), 0f, playerLayer);

        Vector2 realDistance = native.GetDistance(transform.position, playerRB.transform.position).Item1;
        float magnitude = native.GetDistance(transform.position, playerRB.transform.position).Item2;

        if (magnitude <= attackArea) inAttackZone = true;
        else inAttackZone = false;
        if (inAttackZone && canAttack && !isAttacking)
        {
            sprite.flipX = playerRB.position.x - rb.position.x < 0;
            MovementState state;
            state = MovementState.Attacking;
            anim.SetInteger("state", (int)state);
            MakeAttack();
            suspectScript.SetSuspectScale(7f);

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
        enemyFixed = 1;
        rb.velocity = new Vector2(distance.x * JumpForce * 0.66f, distance.y * JumpForce * 0.33f);

        yield return new WaitForSeconds(airAttackCooldown);
        enemyFixed = 0;
    }
}
