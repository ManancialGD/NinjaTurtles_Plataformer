using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D[] coll;
    private SpriteRenderer sprite;
    private Animator anim;
    private float dirX = 0f;
    private bool PlayerInAir;

    public int LastSideWallJumped;

    public bool CanPlayerJump = false;
    private bool PlayerGrounded = true;

    [SerializeField] bool PrintWallState = false;

    // Modificar os parametros de [SerializeField] por código não adianta nada, tem que alterar diretamente do componente
    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] private float InitialMoveSpeed;
    [SerializeField] private float MAX_SPEED;
    [SerializeField] private float jumpForce;
    [SerializeField] private float wallBoost;
    [SerializeField] private float runMultiplier = 1.5f;
    [SerializeField] private float GravityForce;
    private CapsuleCollider2D playerCollider;
    public int isWallSliding;

    public float moveSpeed;

    public int index_debug;

    private enum MovementState { Idle, Walk, Jump, Falling };
    public bool wasGravityChanged;
    public bool WallJumped;
    public bool isWallLocked;

    // Start is called before the first frame update
    private void Start()
    {
        isWallLocked = false;
        WallJumped = false;
        wasGravityChanged = false;
        moveSpeed = InitialMoveSpeed;
        playerCollider = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponents<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (PrintWallState)
        {
            Debug.Log(LastSideWallJumped);
        }
        rb.gravityScale = GravityForce;

        dirX = Input.GetAxisRaw("Horizontal");

        if (Input.GetButton("Run") && GetWallCollision() != 0)
        {

            // Wall Lock

            if (!WallJumped)
            {
                if (dirX > 0 && GetWallCollision() == 2)
                {
                    isWallLocked = true;
                    rb.gravityScale = 0f;
                    rb.velocity = new Vector2(0f, 0f);
                    wasGravityChanged = true;
                }
                else if (dirX < 0 && GetWallCollision() == 1)
                {
                    isWallLocked = true;
                    rb.gravityScale = 0f;
                    rb.velocity = new Vector2(0f, 0f);
                    wasGravityChanged = true;
                }
            }
            else
            {
                WallJumped = false;
            }
        }
        if (Input.GetButton("Run") && Mathf.Abs(dirX) > 0){

            if (dirX == 1)
            {
                if (IsGrounded() == true && GetWallCollision() == 0)
                {
                    if (rb.velocity.x + moveSpeed < MAX_SPEED)
                    {
                        rb.velocity = new Vector2(rb.velocity.x + moveSpeed * runMultiplier, rb.velocity.y);
                    }
                    else
                    {
                        rb.velocity = new Vector2(MAX_SPEED, rb.velocity.y);
                    }
                    anim.speed = 1f;
                }
                else
                {
                    if (rb.velocity.x + moveSpeed < MAX_SPEED)
                    {
                        rb.velocity = new Vector2(rb.velocity.x + moveSpeed, rb.velocity.y);
                    }
                    else
                    {
                        rb.velocity = new Vector2(MAX_SPEED, rb.velocity.y);
                    }
                    anim.speed = runMultiplier;
                }

            }
            else
            {
                if (IsGrounded() == true)
                {
                    if (rb.velocity.x - moveSpeed > -MAX_SPEED)
                    {
                        rb.velocity = new Vector2(rb.velocity.x - moveSpeed * runMultiplier, rb.velocity.y);
                    }
                    else
                    {
                        rb.velocity = new Vector2(-MAX_SPEED, rb.velocity.y);
                    }
                    anim.speed = 1f;
                }
                else
                {
                    if (rb.velocity.x - moveSpeed > -MAX_SPEED)
                    {
                        rb.velocity = new Vector2(rb.velocity.x - moveSpeed, rb.velocity.y);
                    }
                    else
                    {
                        rb.velocity = new Vector2(-MAX_SPEED, rb.velocity.y);
                    }
                    anim.speed = runMultiplier;
                }
            }
        }
        else if (Mathf.Abs(dirX) > 0)
        {

            //index_debug ++;

            //Debug.Log("Correndo - " + index_debug);

            // Wall Slide

            if (dirX > 0 && GetWallCollision() == 2)
            {
                rb.gravityScale = GravityForce;
                if (rb.velocity.y <= -0.4f) rb.velocity = new Vector2(rb.velocity.x, -0.4f);

                wasGravityChanged = true;
            }
            else if (dirX < 0 && GetWallCollision() == 1)
            {
                rb.gravityScale = GravityForce;
                if (rb.velocity.y <= -0.4f) rb.velocity = new Vector2(rb.velocity.x, -0.4f);
                wasGravityChanged = true;
            }
            else
            {
                if (rb.gravityScale != GravityForce)
                {
                    rb.gravityScale = GravityForce;
                    wasGravityChanged = true;
                }
            }

            if (dirX == 1)
            {
                if (rb.velocity.x + moveSpeed < MAX_SPEED)
                {
                    rb.velocity = new Vector2(rb.velocity.x + moveSpeed * runMultiplier, rb.velocity.y);
                }
                else
                {
                    rb.velocity = new Vector2(MAX_SPEED, rb.velocity.y);
                }
                anim.speed = runMultiplier;
            }
            else
            {
                if (rb.velocity.x - moveSpeed > -MAX_SPEED)
                {
                    rb.velocity = new Vector2(rb.velocity.x - moveSpeed * runMultiplier, rb.velocity.y);
                }
                else
                {
                    rb.velocity = new Vector2(-MAX_SPEED, rb.velocity.y);
                }
                anim.speed = runMultiplier;
            }
            anim.speed = 1f;
        }

        if (!wasGravityChanged && rb.gravityScale != GravityForce)
        {
            rb.gravityScale = GravityForce;
        }
        wasGravityChanged = false;

        if(!Input.GetButton("Run")) isWallLocked = false;

        if(isWallLocked) rb.velocity = new Vector2(rb.velocity.x, 0f);



        if (Input.GetButtonDown("Jump") && GetWallCollision() == 1 && LastSideWallJumped != 1)
        {
            LastSideWallJumped = 1;
            rb.gravityScale = GravityForce;
            Debug.Log("wall boost - " + 1);
            rb.velocity = new Vector2(wallBoost / 2, wallBoost * 2);
            sprite.color = new Color(0f, 255f, 0f);
            WallJumped = true;
            if(Input.GetButton("Run")){
                Debug.Log("HHHHHHHHHHHHHHHHHHHHH");
            } 
        }
        else if (Input.GetButtonDown("Jump") && GetWallCollision() == 2 && LastSideWallJumped != 2)
        {
            LastSideWallJumped = 2;
            rb.gravityScale = GravityForce;
            Debug.Log("wall boost - " + 2);
            rb.velocity = new Vector2(-(wallBoost / 2), wallBoost * 2);
            sprite.color = new Color(0f, 0f, 255f);
            WallJumped = true; 
            if(Input.GetButton("Run")) Debug.Log("HHHHHHHHHHHHHHHHHHHHH");

        } else if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            Debug.Log("Ground jump");
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        if (IsGrounded())
        {
            sprite.color = new Color(255f, 255f, 255f);
        }
        else
        {
            //sprite.color = new Color (0.63f, 0.15f, 0.15f);
        }
        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        MovementState state;

        if (rb.velocity.y > 0.1f)
        {
            state = MovementState.Jump;
            sprite.flipX = rb.velocity.x < 0f;
        }
        else if (rb.velocity.y < -0.1f)
        {
            state = MovementState.Falling;
            sprite.flipX = rb.velocity.x < 0f;
        }
        else if (dirX != 0f)
        {
            state = MovementState.Walk;
            sprite.flipX = rb.velocity.x < 0f;
        }
        else
        {
            state = MovementState.Idle;
        }

        //Debug.Log(state);
        anim.SetInteger("state", (int)state);
    }

    public bool IsGrounded()
    {
        if (Physics2D.BoxCast(coll[0].bounds.center, coll[0].bounds.size, 0f, Vector2.down, 0.0f, jumpableGround))
        {
            LastSideWallJumped = 0;
            if (moveSpeed != InitialMoveSpeed) moveSpeed = InitialMoveSpeed;
            PlayerInAir = false;
            return true;
        }
        else
        {
            if (PlayerInAir)
            {
                if (moveSpeed * 1.1f < InitialMoveSpeed)
                {
                    moveSpeed = moveSpeed * 1.1f;
                }
                else
                {
                    moveSpeed = InitialMoveSpeed;
                }
            }
            else
            {
                PlayerInAir = true;
                moveSpeed = 0.01f;
            }

            return false;
        }
    }

    private int GetWallCollision()
    {
        if (IsGrounded()) return -1;
        if (Physics2D.BoxCast(coll[1].bounds.center, coll[1].bounds.size, 0f, Vector2.left, 0.0f, jumpableGround))
        {
            isWallSliding = 1;
            return 1;
        }
        else if (Physics2D.BoxCast(coll[2].bounds.center, coll[2].bounds.size, 0f, Vector2.right, 0.0f, jumpableGround))
        {
            isWallSliding = 2;
            return 2;
        }
        else
        {
            isWallSliding = 0;
            return 0;
        }
    }
}
