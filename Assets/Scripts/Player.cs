using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    Collision coll;
    Rigidbody2D rb;

    [Header("Stats")]
    public float movementSpeed = 10f;
    public float jumpSpeed = 40f;
    public float WallJumpForce = 60f;
    public float slideSpeed = 3f;
    public float wallSlideLerp = 1.5f;

    [Space]

    [Header("Booleans")]
    public bool canMove;
    public bool wallSlide;
    public bool sliding;
    public bool wallJumped;
    private Animator anim;
    private SpriteRenderer sprite;
    private CapsuleCollider2D playerCollider;
    public bool isWallLocked;

    private enum MovementState { Idle, Walk, Jump, Falling, OnWall };
    private string holdOnWallParameter = "HoldOnWall";
    private void Start()
    {
        coll = GetComponent<Collision>();
        rb = GetComponent<Rigidbody2D>();

        isWallLocked = false;
        playerCollider = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");
        Vector2 dir = new Vector2(x, y);

        Move(dir);

        if(xRaw != 0 && !wallSlide)
        {
            sprite.flipX = xRaw < 0;
        }

        if(Input.GetButtonDown("Jump"))
        {
            if (coll.onGround)
            {
                Jump(Vector2.up);
            }
            else if( coll.onWall)
            {
                WallJump();
            }
        }
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f && !wallJumped && wallSlide)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * .5f);
        }

        if( !coll.onGround && coll.onWall)
        {
            
            if( rb.velocity.y > 0)
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
                rb.velocity = new Vector2(rb.velocity.x, -slideSpeed);
            }

        }

        if (coll.onGround)
        {
            wallSlide = false;
            sliding = false;
            wallJumped = false;
        }

        UpdateAnimationState();
    }

    private void Move(Vector2 dir)
    {
        if (!canMove)
        {
            return;
        }

        if (!wallJumped)
        {
            rb.velocity = new Vector2(dir.x * movementSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(dir.x * movementSpeed, rb.velocity.y)), wallSlideLerp * Time.deltaTime);
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
            
            if (coll.onWall)
            {
                state = MovementState.OnWall;
                anim.SetBool(holdOnWallParameter, true);
                sprite.flipX = coll.onRightWall;
            }
            else if (rb.velocity.y > 0.1f || rb.velocity.y < 0.1f && !coll.onGround) 
            {
                state = MovementState.Jump;
                sprite.flipX = rb.velocity.x < 0f;
                anim.SetBool(holdOnWallParameter, false);
            }
            else if (rb.velocity.y < -0.1f && coll.onGround)
            {
                state = MovementState.Falling;
                sprite.flipX = rb.velocity.x < 0f;
                anim.SetBool(holdOnWallParameter, false);
            }
            else if (rb.velocity.x != 0f)
            {
                state = MovementState.Walk;
                sprite.flipX = rb.velocity.x < 0f;
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
