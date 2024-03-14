using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // Initiators
    Collision coll;
    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sprite;
    [SerializeField] private LayerMask jumpableGround;

    static float PLAYER_MAX_AIR_SPEED = 10.0f;


    [Header("Stats")]
    public int damageAmount = 10;
    public float movementSpeed = 10f;
    public float runMultiplier = 1.5f;
    public float jumpSpeed = 40f;
    public Vector2 WallJumpForce;
    public float slideSpeed = 3f;

    public CameraFollow cameraFollow;

    [Space]

    [Header("Booleans")]
    public bool canMove;
    public bool wallSlide;
    public bool sliding;
    public bool wallJumped;
    public bool isRunning;
    public bool isWallLocked;
    public bool isAttackingLeft = false;
    public bool wasAttackingLeft = false;

    private Collision collisionScript;

    private void Start()
    {

        cameraFollow = FindObjectOfType<CameraFollow>();

        coll = GetComponent<Collision>();
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

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
                Jump(jumpSpeed);
            }
            else if (coll.onWall)
            {
                WallJump();
            }
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
                    rb.velocity = new Vector2(rb.velocity.x, 0f);
                    sliding = true;
                }

                // Lock the y position when holding onto the wall
                if (isWallLocked && !coll.onGround)
                {
                    rb.velocity = new Vector2(rb.velocity.x, 0f);
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

    private void LateUpdate()
    {
        if (isAttackingLeft)
        {
            sprite.flipX = false;
        }
        else if (!isAttackingLeft && wasAttackingLeft)
        {
            sprite.flipX = true;
        }

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

        if (coll.onGround)
        {
            // Player no chão

            if (rb.velocity.y < -12f)
            {
                if (rb.velocity.y < -25f) cameraFollow.BoostCamera(new Vector2(rb.velocity.x * 0.3f, -25f * 0.05f - 1f));
                else cameraFollow.BoostCamera(new Vector2(rb.velocity.x * 0.3f, rb.velocity.y * 0.05f - 1f));
            }


            rb.velocity = new Vector2(rb.velocity.x + dir.x * currentSpeed * Time.deltaTime, rb.velocity.y);
        }
        else if (!coll.onGround)
        {
            // Player no ar
            if (Mathf.Abs(dir.x) > 0 && Mathf.Abs(rb.velocity.x) < PLAYER_MAX_AIR_SPEED) rb.velocity = new Vector2(rb.velocity.x + dir.x * currentSpeed * Time.deltaTime, rb.velocity.y);
        }
    }


    private void Jump(float yForce)
    {
        Debug.Log("Jumped");
        rb.velocity = new Vector2(rb.velocity.x, yForce);
    }

    private void WallJump()
    {

        //StartCoroutine(DisableMovement(0.05f));

        int wallSide = 0;
        if (coll.onLeftWall) wallSide = 1;
        else if (coll.onRightWall) wallSide = -1;

        wallJumped = true;
        rb.velocity = new Vector2(WallJumpForce.x * wallSide, WallJumpForce.y);

        float inputHorizontal = Input.GetAxis("Horizontal");
        int dirX;

        if (inputHorizontal > 0)
        {
            dirX = 1;
        }
        else if (inputHorizontal < 0)
        {
            dirX = -1;
        }
        else
        {
            dirX = 0;
        }

        if (wallSide == dirX || dirX == 0)
        {
            cameraFollow.BoostCamera(new Vector2(WallJumpForce.x * wallSide * 1.3f, WallJumpForce.y * 1.3f));
            cameraFollow.SetCameraReaction(0f, 3f);
        } else {
            cameraFollow.SetCameraReaction(0f, 2f);
        }
        return;
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