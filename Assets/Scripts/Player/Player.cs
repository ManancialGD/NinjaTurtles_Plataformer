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
    SpriteRenderer sprite;

    static float PLAYER_MAX_AIR_SPEED = 10.0f;


    [Header("Stats")]
    public int damageAmount = 10;
    public float movementSpeed = 10f;
    public float movementLerp = 4f;
    public float jumpSpeed = 40f;
    public float WallJumpForce = 60f;
    public float slideSpeed = 3f;
    public float wallJumpLerp = 4f;

    [Space]

    [Header("Booleans")]
    public bool canMove;
    public bool wallSlide;
    public bool sliding;
    public bool wallJumped;
    public bool isWallLocked;

    private Collision collisionScript;

    private void Start()
    {
        coll = GetComponent<Collision>();
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        collisionScript = GetComponent<Collision>();
    }

    void Update()
    {
        //Debug.Log("PlayerHealth = " + PlayerHealth);// porquê que isto não está a ser printado??

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");
        Vector2 dir = new Vector2(x, y);

        Move(dir);

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
    }

    private void Move(Vector2 dir)
    {
        if (!canMove) return;


        float currentSpeed = movementSpeed;

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

    public int GetDamage()
    {
        return damageAmount;
    }
}
