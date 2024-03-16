using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // Initiators
    PlayerParticles playerParticles;
    public float playerAttackCooldown;
    public float gravityForce;
    public Sprite BallSprite;
    Collision coll;
    Rigidbody2D rb;
    Animator anim;

    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] Transform CollisionDetectorGroup;


    [Header("Stats")]
    public Vector2 playerAttackForce;
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
    public PlayerAnimation playerAnimation;

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
    public bool isPlayerAttacking = false;
    bool isPlayerDownAttacking = false;
    public int airAttack_Down_Damage;

    private void Start()
    {
        layerId = LayerMask.NameToLayer("Enemys");
        playerSpeed = new Vector2(groundAcceleration, 0f);

        playerParticles = FindObjectOfType<PlayerParticles>();
        cameraFollow = FindObjectOfType<CameraFollow>();
        playerAnimation = FindObjectOfType<PlayerAnimation>();

        coll = GetComponent<Collision>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        collisionScript = GetComponent<Collision>();

    }

    void Update()
    {

        CheckForAirAttackExplosion();
        //Debug.Log(GetFramesUntilCollision(true, false));
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

        if (!isPlayerAttacking)
        {
            rb.velocity = new Vector2(rb.velocity.x, yForce);
        }
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
            cameraFollow.BoostCamera(new Vector2(WallJumpForce.x * wallSide * 1.5f, WallJumpForce.y * 2f));
            cameraFollow.SetCameraReaction(0.1f, 0.7f);
        }
        else
        {
            cameraFollow.SetCameraReaction(0.1f, 0.7f);
        }
        return;
    }

    public int GetDamage()
    {
        return damageAmount;
    }


    public void EnablePlayerAttack()
    {
        isPlayerAttacking = true;
        playerAnimation.SetAttackCooldown(playerAttackCooldown);
        return;
    }
    public void DisablePlayerAttack()
    {
        isPlayerAttacking = false;
        return;
    }

    public Vector2 GetPlayerPosition()
    {
        rb = GetComponent<Rigidbody2D>();
        return new Vector2(rb.position.x, rb.position.y);
    }

    public void BoostPlayer(Vector2 force)
    {
        rb.velocity = new Vector2(rb.velocity.x + force.x, rb.velocity.y + force.y);
        return;
    }

    public float GetPlayerDistanceFromGround()
    {
        rb = GetComponent<Rigidbody2D>();
        BoxCollider2D[] box_collider = GetComponents<BoxCollider2D>();

        bool onGround = false;
        float distance = 0f;
        while (!onGround)
        {
            Vector2 newColliderPosition = new Vector2(rb.position.x, rb.position.y - distance);
            if (!Physics2D.BoxCast(newColliderPosition, box_collider[0].bounds.size, 0f, Vector2.down, 0.0f, jumpableGround)) distance += 0.1f;
            else onGround = true;
        }
        return distance;
    }


    private int GetFramesUntilCollision(bool show, bool jumping)
    {

        Rigidbody2D thisRB = GetComponent<Rigidbody2D>();
        int maxTrys = 75;

        float velocityX = thisRB.velocity.x;
        float velocityY = thisRB.velocity.y;

        float posX = thisRB.position.x;
        float posY = thisRB.position.y;

        BoxCollider2D[] boxColliders = GetComponents<BoxCollider2D>();

        if (jumping) velocityY = jumpSpeed;

        if (show)
        {

            foreach (Transform child in CollisionDetectorGroup)
            {
                Destroy(child.gameObject);
            }

        }

        float newposX = posX;
        float newposY = posY;

        for (int i = 0; i < maxTrys; i++)
        {

            newposX += velocityX * 0.08f;
            newposY += velocityY * 0.08f - (i * (gravityForce * 0.08f));

            Vector2 newColliderPosition = new Vector2(newposX, newposY);

            if (show)
            {
                GameObject newSpriteObject = new GameObject("CollisionDetector_" + i);
                newSpriteObject.transform.SetParent(CollisionDetectorGroup);
                SpriteRenderer spriteRenderer = newSpriteObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = BallSprite;
                newSpriteObject.transform.position = newColliderPosition;
                newSpriteObject.transform.localScale = new Vector2(0.3f, 0.3f);
            }

            if (Physics2D.BoxCast(newColliderPosition, boxColliders[0].bounds.size, 0f, Vector2.right, 0.0f, jumpableGround))
            {
                return i;
            }
        }

        return maxTrys;

    }
    // ------------ Air Attacks --------------

    public void AirAttack_Down()
    {
        isPlayerDownAttacking = true;
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(rb.velocity.x * 0.2f, -15f);
        return;
    }
    void CheckForAirAttackExplosion()
    {
        if (coll.onGround && isPlayerDownAttacking)
        {
            rb = GetComponent<Rigidbody2D>();
            isPlayerDownAttacking = false;
            GroundImpact(new Vector2(rb.position.x, rb.position.y), 7f);
        }
    }
    void GroundImpact(Vector2 position, float impactArea)
    {
        GameObject[] todosObjetos = GameObject.FindObjectsOfType<GameObject>();



        foreach (GameObject obj in todosObjetos)
        {
            if (obj.CompareTag("Enemy"))
            {

                Transform objectTransform = obj.GetComponent<Transform>();
                float distanceLast = (position.x - objectTransform.position.x) + (position.y - objectTransform.position.y);

                if (Mathf.Abs(distanceLast) <= impactArea) // Combat Mode
                {

                    EnemyHP enemyHP;
                    enemyHP = FindObjectOfType<EnemyHP>();

                    enemyHP.TakeDamage(airAttack_Down_Damage, new Vector2(10f, 4f), 2f);

                }

            }
        }
    }

    // ---------------------------------------

}