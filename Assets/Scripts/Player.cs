using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // Initiators
    Collision coll;
    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sprite;
    GameObject playerOBJ;

    static float PLAYER_MAX_AIR_SPEED = 10.0f;


    [Header("Stats")]
    public int damageAmount = 10;
    public float movementSpeed = 10f;
    public float movementLerp = 4f;
    public float runMultiplier = 1.5f;
    public float jumpSpeed = 40f;
    public float WallJumpForce = 60f;
    public float slideSpeed = 3f;
    public float wallJumpLerp = 4f;

    public float PlayerHealth;


    [Space]

    [Header("Booleans")]
    public bool canMove;
    public bool wallSlide;
    public bool sliding;
    public bool wallJumped;
    public bool isRunning;
    public bool isWallLocked;
    public bool canFlip;

    private Collision collisionScript;

    // Animation variables
    private enum MovementState { Idle, Walk, Jump, Falling, OnWall, Attacking, LeftAttacking };
    private string holdOnWallParameter = "HoldOnWall";
    private void Start()
    {
        coll = GetComponent<Collision>();
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        collisionScript = GetComponent<Collision>();
        playerOBJ = GameObject.Find("Player");
    }

    void Update()
    {
        //Debug.Log("PlayerHealth = " + PlayerHealth);// porquê que isto não está a ser printado??

        if (PlayerHealth <= 0)
        {
            if (playerOBJ) PlayerDied();
            return;
        }

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");
        Vector2 dir = new Vector2(x, y);

        Move(dir);

        if (xRaw != 0 && !wallSlide && canFlip)
        {
            sprite.flipX = xRaw < 0;
        }
        if (Input.GetButtonDown("Run") && coll.onGround)
        {
            isRunning = true;
        }
        else if (Input.GetButtonUp("Run"))
        {
            isRunning = false;
        }
        if (coll.onWall && !coll.onGround) isRunning = false;

        if (Input.GetButtonDown("HoldOnWall"))
        {
            isWallLocked = true;
        }
        else if (Input.GetButtonUp("HoldOnWall"))
        {
            isWallLocked = false;
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (coll.onGround)
            {
                Jump(Vector2.up);
            }
            else if (coll.onWall)
            {
                WallJump();
            }
        }
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f && !wallJumped && wallSlide)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * .5f);
        }

        if (!coll.onGround && coll.onWall)
        {
            if (rb.velocity.y > 0)
            {
                wallSlide = false;
                return;
            }

            if (coll.onWall) wallSlide = true;
            else wallSlide = false;

            if (wallSlide)
            {
                if (!sliding)
                {
                    rb.velocity = new Vector2(rb.velocity.x, 0);
                    sliding = true;
                }

                // Lock the y position when holding onto the wall
                if (isWallLocked && !coll.onGround)
                {
                    rb.velocity = new Vector2(rb.velocity.x, 0);
                }
                else
                {
                    rb.velocity = new Vector2(rb.velocity.x, -slideSpeed);
                }
            }

        }

        if (coll.onGround)
        {
            wallSlide = false;
            sliding = false;
            wallJumped = false;
        }
        else if (coll.onWall) wallJumped = false;

        UpdateAnimationState();
    }

    private void Move(Vector2 dir)
    {
        if (!canMove) return;


        float currentSpeed = movementSpeed;

        if (isRunning)
        {   // ( isRunning && coll.onGround ) ?
            // Caso ele o player só consiga acelerar no chão;
            currentSpeed = movementSpeed * runMultiplier;
        }

        if (coll.onGround && !coll.onWall)
        {
            // Player no chão
            rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(dir.x * currentSpeed, rb.velocity.y)), movementLerp * Time.deltaTime);
        }
        else if (!coll.onGround)
        {
            // Player no ar
            if (Mathf.Abs(dir.x) > 0 && rb.velocity.x < PLAYER_MAX_AIR_SPEED) rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(rb.velocity.x + (dir.x * currentSpeed), rb.velocity.y)), wallJumpLerp * Time.deltaTime);
        }
    }


    private void Jump(Vector2 dir)
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        rb.AddForce(dir * jumpSpeed, ForceMode2D.Impulse);
    }

    private void WallJump()
    {
        StartCoroutine(DisableMovement(0.05f));

        Vector2 wallDir = coll.onRightWall ? Vector2.left : Vector2.right;
        Jump((Vector2.up + wallDir * WallJumpForce));

        wallJumped = true;
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
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
        else if (rb.velocity.x != 0f)
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
        if (state == MovementState.Attacking || state == MovementState.LeftAttacking)
        {
            sprite.flipX = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("DamagePlayer"))
        {
            Destroy(gameObject);
        }
    }

    public void CanItFlip(bool cantFlip)
    {
        if (cantFlip) canFlip = false;
        else canFlip = true;
    }

    public int GetDamage()
    {
        return damageAmount;
    }

    public int DamagePlayer(float damage)
    {
        PlayerHealth -= damage;

        if (PlayerHealth <= 0)
        {
            PlayerDied();
        }
        return 1;
    }

    int PlayerDied()
    {
        if (playerOBJ)
        {
            Destroy(playerOBJ);
            return 1;
        }
        else
        {
            return 0;
        }
    }

}