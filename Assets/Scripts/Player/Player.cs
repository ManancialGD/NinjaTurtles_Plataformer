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

    [SerializeField] private LayerMask jumpableGround;


    [Header("Stats")]
    public int damageAmount = 10;
    public float jumpSpeed = 40f;
    public Vector2 WallJumpForce;
    public float slideSpeed = 3f;
    float PLAYER_MAX_GROUND_VELOCITY = 5f;
    float PLAYER_MAX_AIR_VELOCITY = 5f;
    public float groundAcceleration;
    public float airAcceleration;
    public float groundFriction;
    public float airFriction;
    public float momentumLost;

    Vector2 playerSpeed = new Vector2(0f, 0f);

    public CameraFollow cameraFollow;

    [Space]

    [Header("Booleans")]
    public bool canMove;
    public bool wallSlide;
    public bool sliding;
    public bool wallJumped;
    public bool isWallLocked;

    private Collision collisionScript;
    int layerId;
    Vector2 dir_History = new Vector2(0f, 0f);

    private void Start()
    {
        layerId = LayerMask.NameToLayer("Enemys");
        playerSpeed = new Vector2(groundAcceleration, 0f);

        cameraFollow = FindObjectOfType<CameraFollow>();

        coll = GetComponent<Collision>();
        rb = GetComponent<Rigidbody2D>();
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

    private void Move(Vector2 dir)
    {
        if (!canMove) return;

        if (coll.onGround)
        {   // Player on Ground

            if (rb.velocity.y < -12f)
            {
                if (rb.velocity.y < -25f) cameraFollow.BoostCamera(new Vector2(rb.velocity.x * 0.1f, -25f * 0.05f));
                else cameraFollow.BoostCamera(new Vector2(rb.velocity.x * 0.1f, rb.velocity.y * 0.05f));
            }

            if (Mathf.Abs(dir.x) > 0)
            {
                if (coll.onGround)
                {
                    //if (playerSpeed.x < PLAYER_MAX_GROUND_VELOCITY) playerSpeed = new Vector2(playerSpeed.x + groundAcceleration * dir.x * Time.deltaTime, playerSpeed.y);

                }
                else if (!coll.onGround && !coll.onWall)
                {

                }

                //if (Mathf.Abs(dir.x) == 0 && playerSpeed.x > 0f) playerSpeed = new Vector2(playerSpeed.x - momentumLost * Time.deltaTime, playerSpeed.y);
                //if (playerSpeed.x < 0f) playerSpeed = new Vector2(0f, playerSpeed.y);
            }

            //ground friction
            if (dir.x == 0 && !coll.onWall) rb.velocity = new Vector2(rb.velocity.x - groundFriction * Time.deltaTime, rb.velocity.y);
            //air friction (currently none)
            //else if (dir.x == 0 && !coll.onWall && !coll.onGround) rb.velocity = new Vector2(rb.velocity.x - airFriction * Time.deltaTime, rb.velocity.y);

            if (rb.velocity.x > 0 && dir.x < 0 || rb.velocity.x < 0 && dir.x > 0)
            { // quando o jogador troca de direcao
                //rb.velocity = new Vector2(-rb.velocity.x * 0.9f + dir.x * playerSpeed.x * Time.deltaTime, rb.velocity.y); (Boost)
                rb.velocity = new Vector2(rb.velocity.x + dir.x * playerSpeed.x * Time.deltaTime, rb.velocity.y);
            }
            else if (rb.velocity.x < PLAYER_MAX_GROUND_VELOCITY && Mathf.Abs(dir.x) > 0 && rb.velocity.x > -PLAYER_MAX_GROUND_VELOCITY && Mathf.Abs(dir.x) > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x + dir.x * playerSpeed.x * Time.deltaTime, rb.velocity.y);
            }


        }
        else if (!coll.onGround && !coll.onWall)
        {   // Player no air

            if (dir.x == 0 && coll.onGround && !coll.onWall) rb.velocity = new Vector2(rb.velocity.x * groundFriction * Time.deltaTime, rb.velocity.y);

            if (rb.velocity.x > PLAYER_MAX_AIR_VELOCITY && dir.x < 0 || rb.velocity.x < -PLAYER_MAX_AIR_VELOCITY && dir.x > 0)
            { // quando o jogador troca de direcao em excesso de velocidade
                rb.velocity = new Vector2(rb.velocity.x + dir.x * playerSpeed.x * Time.deltaTime, rb.velocity.y);
            }
            else if (rb.velocity.x < PLAYER_MAX_GROUND_VELOCITY && rb.velocity.x > -PLAYER_MAX_GROUND_VELOCITY && Mathf.Abs(dir.x) > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x + dir.x * playerSpeed.x * Time.deltaTime, rb.velocity.y);
            }
        }
        else if (!coll.onGround && coll.onWall)
        {   // Player on wall (por agora igual a "Player on air")
            if (Mathf.Abs(dir.x) > 0 && Mathf.Abs(rb.velocity.x) < PLAYER_MAX_AIR_VELOCITY) rb.velocity = new Vector2(rb.velocity.x + dir.x * Time.deltaTime, rb.velocity.y);
        }
    }


    private void Jump(float yForce)
    {
        anim = GetComponent<Animator>();
        Debug.Log("anim = " + anim.GetInteger("state"));
        if (anim.GetInteger("state") != 5 && anim.GetInteger("state") != 6) rb.velocity = new Vector2(rb.velocity.x, yForce);
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
            cameraFollow.SetCameraReaction(cameraFollow.MAX_CAMERA_REACTION, 0f);
            cameraFollow.BoostCamera(new Vector2(WallJumpForce.x * wallSide * 1.5f, WallJumpForce.y * 1.6f));
            cameraFollow.SetCameraReaction(0f, 0f);
        }
        else
        {
            cameraFollow.SetCameraReaction(0f, 0f);
        }
        return;
    }

    public int GetDamage()
    {
        return damageAmount;
    }

}