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
    public int thisPlayerID;
    PlayerParticles playerParticles;
    public float playerAttackCooldown;
    public float gravityForce;
    public Sprite BallSprite;
    Collision coll;
    public Rigidbody2D rb;
    Animator anim;

    [SerializeField] private LayerMask jumpableGround;

    [SerializeField] Transform CollisionDetectorGroup;


    [Header("Stats")]
    Vector2 playerAttackForce;
    int damageAmount;
    float jumpSpeed;
    float jumpPressedBoost;
    float lastJumpCooldown;
    Vector2 WallJumpForce;
    public float slideSpeed;
    float PLAYER_MAX_GROUND_VELOCITY = 5f;
    float PLAYER_MAX_AIR_VELOCITY = 5f;
    public float groundAcceleration;
    public float airAcceleration;
    public float groundFriction;

    (bool, float) dashDetector = (false, 0); // (rightSide, LastPressCooldown)

    Vector2 playerSpeed = new Vector2(0f, 0f);

    public CameraFollow cameraFollow;
    NativeInfo native;

    [Space]

    [Header("Booleans")]
    public bool canMove;
    public bool wallSlide;
    public bool sliding;
    private float jumped;
    public float wallJumped;
    bool isWallLocked = false;

    public Vector2 attackVelocityBoost;

    private Collision collisionScript;
    int layerId;
    Vector2 dir_History = new Vector2(0f, 0f);
    public bool isPlayerDownAttacking;
    public int airAttack_Down_Damage;
    BasicPlayerParticles basicPlayerParticles;
    Player_GroundSlamParticles groundSlamParticles;
    ThrowRockScript throwRockScript;
    PlayerHP playerHP;
    public Vector2 dir;
    bool playerDashing = false;

    LeoAttacks leoAttacks;


    bool loaded;

    private void Start()
    {
        loaded = false;
        native = FindObjectOfType<NativeInfo>();
        UpdatePlayerInfo(thisPlayerID);
        
        leoAttacks = GetComponent<LeoAttacks>();

        layerId = LayerMask.NameToLayer("Enemys");
        playerSpeed = new Vector2(groundAcceleration, 0f);
        playerHP = GetComponent<PlayerHP>();

        basicPlayerParticles = FindObjectOfType<BasicPlayerParticles>();
        groundSlamParticles = FindObjectOfType<Player_GroundSlamParticles>();
        playerParticles = FindObjectOfType<PlayerParticles>();
        cameraFollow = FindObjectOfType<CameraFollow>();
        throwRockScript = GetComponent<ThrowRockScript>();


        coll = GetComponent<Collision>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        collisionScript = GetComponent<Collision>();
        isPlayerDownAttacking = false;

    }

    void FixedUpdate()
    {
        if (thisPlayerID != native.currentPlayerID) return; // Não é o player a ser controlado

        if (sliding)
        {

            int particlesDisplayed = Mathf.Abs((int)UnityEngine.Random.Range(0f, 1.2f));

            int sideWall = -1;
            string sideName = "right";
            float[] sideCorrection = { -32 * 5, UnityEngine.Random.Range(0, 32 * 5) };
            if (coll.onLeftWall)
            {
                sideName = "left";
                sideWall = 1;
                sideCorrection[0] = 32 * 5;
            }

            float[] multiplierX = { 0.0f, 1f };
            float[] multiplierY = { 0f, 0.2f };

            if (particlesDisplayed > 0) basicPlayerParticles.CreateParticle(particlesDisplayed, sideName, new Vector2(sideWall * 0.4f, 0f), multiplierX, multiplierY, sideCorrection, Color.white);

        }
    }
    void Update()
    {
        if (jumped > 0) jumped += Time.deltaTime;
        if (jumped >= 1) jumped = 0;
        if (wallJumped > 0) wallJumped += Time.deltaTime;
        if (wallJumped >= 1) wallJumped = 0;


        if (lastJumpCooldown > 0f) lastJumpCooldown += Time.deltaTime;

        if (lastJumpCooldown >= 1f) lastJumpCooldown = 0f;

        CheckForAirAttackExplosion();

        if (thisPlayerID != native.currentPlayerID)
        {
            if (throwRockScript.IsAiming()) throwRockScript.StopAiming();
            return;
        } // Não é o player a ser controlado

        DashDetection();

        //UpdateCheckpoints();

        //Debug.Log(GetFramesUntilCollision(true, false));
        //Debug.Log("PlayerHealth = " + PlayerHealth);// porquê que isto não está a ser printado??

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");


        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");
        dir = new Vector2(x, y);

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
            if (coll.onGround && jumped <= 0)
            {
                if (playerDashing) Jump(jumpSpeed + native.playerDashJumpForce[thisPlayerID - 1]);
                else Jump(jumpSpeed);
            }
            else if (coll.onWall && wallJumped <= 0)
            {
                WallJump();
            }
            else if (lastJumpCooldown <= 1f && lastJumpCooldown > 0f) //1sec
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + (1f - lastJumpCooldown) * 0.02f);
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
            if (wallJumped >= 0.05f) wallJumped = 0;
            if (jumped >= 0.05f) jumped = 0;
        }
        else if (coll.onWall)
        {
            if (wallJumped >= 0.05f) wallJumped = 0;
            if (jumped >= 0.05f) jumped = 0;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("start aiming");
            throwRockScript.StartAiming();
        }
        if (Input.GetKeyUp(KeyCode.Q))
        {
            throwRockScript.StopAiming();
            Debug.Log("stop aiming");
        }

    }

    private void Move(Vector2 dir)
    {
        if (!canMove || leoAttacks.isAttacking) return;

        if (native.isPlayerAttachToAnyLamp())
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            Debug.Log("Player velocity = " + rb.velocity);
        }

        if (coll.onGround)
        {   // Player on Ground

            if (rb.velocity.y < -50f)
            { // Particulas
                Debug.Log("fall velocity = " + rb.velocity.y);
                int particlesDisplayed = (int)(Mathf.Abs(UnityEngine.Random.Range(rb.velocity.y * 0.1f, rb.velocity.y * 0.3f)) / 35);

                //playerParticles.CreateParticle(1f, "down", new Vector2(rb.velocity.x * UnityEngine.Random.Range(0.5f, 0.8f) + sideCorrection, -rb.velocity.y * UnityEngine.Random.Range(0.1f, 0.4f)));

                float[] multiplierX = { 0.3f, 0.8f };
                float[] multiplierY = { 0.1f, 0.4f };
                float[] sideCorrection = { -96f, 96f };

                basicPlayerParticles.CreateParticle(particlesDisplayed, "down", new Vector2(rb.velocity.x, -rb.velocity.y), multiplierX, multiplierY, sideCorrection, Color.white);
            }


            if (rb.velocity.y < -400f && !isPlayerDownAttacking)
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
            else if (rb.velocity.x < PLAYER_MAX_GROUND_VELOCITY && dir.x > 0 || rb.velocity.x > -PLAYER_MAX_GROUND_VELOCITY && dir.x < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x + dir.x * playerSpeed.x * Time.deltaTime, rb.velocity.y);

            }


        }
        else if (!coll.onGround && !coll.onWall)
        {   // Player no air

            if (dir.x == 0 && coll.onGround && !coll.onWall) rb.velocity = new Vector2(rb.velocity.x * groundFriction * Time.deltaTime, rb.velocity.y);

            if (rb.velocity.x > PLAYER_MAX_AIR_VELOCITY && dir.x < 0 || rb.velocity.x < -PLAYER_MAX_AIR_VELOCITY && dir.x > 0)
            { // quando o jogador troca de direcao em excesso de velocidade
                rb.velocity = new Vector2(rb.velocity.x + dir.x * airAcceleration * Time.deltaTime, rb.velocity.y);
            }
            else if (rb.velocity.x < PLAYER_MAX_AIR_VELOCITY && dir.x > 0 || rb.velocity.x > -PLAYER_MAX_AIR_VELOCITY && dir.x < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x + dir.x * airAcceleration * Time.deltaTime, rb.velocity.y);
            }

        }
        else if (!coll.onGround && coll.onWall)
        {   // Player on wall (por agora igual a "Player on air")
            if (Mathf.Abs(dir.x) > 0 && Mathf.Abs(rb.velocity.x) < PLAYER_MAX_AIR_VELOCITY) rb.velocity = new Vector2(rb.velocity.x + dir.x * Time.deltaTime, rb.velocity.y);
        }
    }


    private void Jump(float yForce)
    {

        if (!leoAttacks.isAttacking && jumped <= 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, yForce);

            int particlesDisplayed = (int)UnityEngine.Random.Range(3f, 6f);

            float[] multiplierX = { 0.05f, 0.8f };
            float[] multiplierY = { 0.05f, 0.6f };

            float[] sideCorrection = { -3.2f, 3.2f };

            basicPlayerParticles.CreateParticle(particlesDisplayed, "down", new Vector2(rb.velocity.x, yForce), multiplierX, multiplierY, sideCorrection, Color.white);
            if (sliding) sliding = false;
            lastJumpCooldown = 0.001f;
            jumped = 0.001f;

        }
    }

    private void WallJump()
    {

        //StartCoroutine(DisableMovement(0.05f));

        int wallSide = 0;
        if (coll.onLeftWall) wallSide = 1;
        else if (coll.onRightWall) wallSide = -1;

        wallJumped = 0.001f;
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
            cameraFollow.BoostCamera(new Vector2(WallJumpForce.x * wallSide * 1.5f, WallJumpForce.y));
            cameraFollow.SetCameraReaction(0.1f, 0.7f);
        }
        else
        {
            cameraFollow.SetCameraReaction(0.1f, 0.7f);
        }


        // Particulas
        int particlesDisplayed = Mathf.Abs((int)UnityEngine.Random.Range(4f, 7f));
        float[] multiplierX = { 0.05f, 0.7f };
        float[] multiplierY = { 0.05f, 0.6f };

        float[] sideCorrection = { -1.6f, 1.6f };

        string sideName = "right";
        if (coll.onLeftWall) sideName = "left";

        if (sliding) sliding = false;

        basicPlayerParticles.CreateParticle(particlesDisplayed, sideName, new Vector2(rb.velocity.x / Mathf.Abs(rb.velocity.x) * WallJumpForce.x, WallJumpForce.y), multiplierX, multiplierY, sideCorrection, Color.white);

        return;
    }

    public int GetDamage()
    {
        return damageAmount;
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

        float error_margin = 0.1f;

        while (!onGround)
        {
            Vector2 newColliderPosition = new Vector2(box_collider[0].bounds.center.x, box_collider[0].bounds.center.y - distance);
            if (!Physics2D.BoxCast(newColliderPosition, box_collider[0].bounds.size, 0f, Vector2.down, 0.0f, jumpableGround)) distance += error_margin;
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
            GroundImpact(new Vector2(rb.position.x, rb.position.y), new Vector2(rb.velocity.x, rb.velocity.y), new Vector2(7f, 1.5f));
            groundSlamParticles.DisplayGroundSlamParticles(rb.transform.position);
            playerHP.ConsumeStamina(native.staminaUse_GroundSlam);
        }
    }
    void GroundImpact(Vector2 position, Vector2 velocity, Vector2 impactArea)
    {
        GameObject[] todosObjetos = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in todosObjetos)
        {
            if (obj.CompareTag("Enemy"))
            {

                Transform objectTransform = obj.GetComponent<Transform>();
                Vector2 distanceLast = new Vector2(position.x - objectTransform.position.x, position.y - objectTransform.position.y);

                if (Mathf.Abs(distanceLast.x) <= impactArea.x && Mathf.Abs(distanceLast.y) <= impactArea.y) // Combat Mode
                {

                    EnemyHP enemyHP;
                    enemyHP = FindObjectOfType<EnemyHP>();

                    enemyHP.TakeDamage(airAttack_Down_Damage, new Vector2(0f, 0f), 2f);
                    //distanceLast.x / Mathf.Abs(distanceLast.x) * 10, distanceLast.y / Mathf.Abs(distanceLast.y) * 4f

                }

            }
        }
    }

    // ---------------------------------------

    // --------------- Native ----------------

    public void UpdatePlayerInfo(int playerID)
    {
        playerID -= 1; // Escala de 1 - 4 // Index de 0 - 3
        Debug.Log("UpdatePlayerInfo( " + playerID + " )");

        groundAcceleration = native.playerGroundAcceleration[playerID];
        Debug.Log("1");
        airAcceleration = native.playerAirAcceleration[playerID];
        groundFriction = native.PlayerGroundFriction[playerID];
        damageAmount = native.playerDamage[playerID];
        slideSpeed = native.playerSlideSpeed[playerID];
        jumpSpeed = native.playerJumpForce[playerID];
        PLAYER_MAX_GROUND_VELOCITY = native.PLAYER_MAX_GROUND_VELOCITY[playerID];
        PLAYER_MAX_AIR_VELOCITY = native.PLAYER_MAX_AIR_VELOCITY[playerID];
        airAttack_Down_Damage = native.playerGroundSlamDamage[playerID];
        attackVelocityBoost = native.playerAttackVelocityBoost[playerID];
        playerAttackForce = native.playerAttackForce[playerID];
        WallJumpForce = native.playerWallJumpForce[playerID];

        Debug.Log("Player " + (playerID + 1) + " information updated");

        return;
    }


    //----------------------------------------

    void UpdateCheckpoints()
    {
        if (native.GetCheckpointsAmount() <= 0)
        {
            if (coll.onGround) native.CreateTargetCheckpoint(native.GetSelectedPlayerPosition());
            return;
        }

        Vector2 nearestCheckpoint;
        int nearestCheckpointID;

        (nearestCheckpoint, nearestCheckpointID) = native.GetDistanceFromNearestCheckpoint(native.GetSelectedPlayerPosition());
        if (Mathf.Abs(nearestCheckpoint.x) + Mathf.Abs(nearestCheckpoint.y) > 2f && coll.onGround)
        {
            native.CreateTargetCheckpoint(native.GetSelectedPlayerPosition());
        }

    }


    bool DashDetection()
    {
        if (dashDetector.Item2 < 0)
        {
            dashDetector.Item2 += Time.deltaTime;
            if (dashDetector.Item2 > 0) dashDetector.Item2 = 0;
            return false;
        }
        else if (dashDetector.Item2 == 0)
        {
            if (Input.GetKeyDown(KeyCode.A)) dashDetector = (false, 0.001f);
            else if (Input.GetKeyDown(KeyCode.D)) dashDetector = (true, 0.001f);
        }
        else if (dashDetector.Item2 <= 0.2f && coll.onGround && playerHP.playerStamina >= native.staminaUse_Dash)
        {
            if (Input.GetKeyDown(KeyCode.A) && dashDetector.Item1 == false)
            {
                rb.velocity = new Vector2(-native.playerDashSpeed[thisPlayerID - 1], rb.velocity.y);
                playerDashing = true;
                StartCoroutine(StopDash(native.playerDashTime[thisPlayerID - 1]));
                dashDetector = (false, -0.5f);
                playerHP.ConsumeStamina(native.staminaUse_Dash);
                return true;
            }
            else if (Input.GetKeyDown(KeyCode.D) && dashDetector.Item1 == true)
            {
                rb.velocity = new Vector2(native.playerDashSpeed[thisPlayerID - 1], rb.velocity.y);
                playerDashing = true;
                StartCoroutine(StopDash(native.playerDashTime[thisPlayerID - 1]));
                dashDetector = (true, -0.5f);
                playerHP.ConsumeStamina(native.staminaUse_Dash);
                return true;
            }
        }
        else if (dashDetector.Item2 >= 0.2f)
        {
            dashDetector.Item2 = 0;
            return false;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.A)) dashDetector = (false, 0.001f);
            else if (Input.GetKeyDown(KeyCode.D)) dashDetector = (true, 0.001f);
            else if (dashDetector.Item2 != 0) dashDetector.Item2 += Time.deltaTime;
        }
        return false;
    }

    private IEnumerator StopDash(float delay)
    {
        yield return new WaitForSeconds(delay);
        rb.velocity = new Vector2(0f, rb.velocity.y);
        playerDashing = false;
    }

}