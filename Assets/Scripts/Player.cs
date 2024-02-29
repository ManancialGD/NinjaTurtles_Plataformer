using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] Sprite CollisionDetectorSprite;
    [SerializeField] Transform CollisionDetectorGroup;
    private Rigidbody2D rb;
    private BoxCollider2D[] coll;
    private SpriteRenderer sprite;
    private Animator anim;
    private float dirX = 0f;
    private bool PlayerInAir;

    private int wallSideJumped_history = 0;

    public bool PrintWallState = false;

    // Modificar os parametros de [SerializeField] por código não adianta nada, tem que alterar diretamente do componente
    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] private float InitialMoveSpeed;
    [SerializeField] private float MAX_SPEED;
    [SerializeField] private float jumpForce;
    [SerializeField] private float wallBoost;
    [SerializeField] private float runMultiplier = 1.5f;
    [SerializeField] private float GravityForce;
    private CapsuleCollider2D playerCollider;

    public float moveSpeed;


    private enum MovementState { Idle, Walk, Jump, Falling, OnWall };
    private string holdOnWallParameter = "holdOnWall";
    public bool isWallLocked;
    public float groundFriction;

    private bool Key_Jump;
    public float WallSlideSpeed;
    private float PlayerFinalRotation;
    public float ROTATION_RATE_DEFAULT;

    // Start is called before the first frame update
    private void Start()
    {
        PlayerFinalRotation = 0f;
        Key_Jump = false;
        isWallLocked = false;
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
            Debug.Log(wallSideJumped_history);
        }

        KeyDetection();

        GroundDetection();
        PlayerMovement();
        WallDetection();

        UpdateAnimationState();
        PlayerAutoRotate();
        //Debug.Log(GetFramesUntilCollision(false));

    }

    public void PlayerMovement()
    {
        dirX = Input.GetAxisRaw("Horizontal");

        if (isWallLocked) return;

        if (!PlayerInAir)
        { // IN GROUND
            moveSpeed = InitialMoveSpeed;

            if (Key_Jump && !PlayerInAir)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }

            if (!Input.GetButton("Run"))
            {
                if (rb.velocity.x + moveSpeed <= MAX_SPEED && dirX == 1)
                {
                    rb.velocity = new Vector2(rb.velocity.x + moveSpeed * dirX, rb.velocity.y);
                }
                else if (rb.velocity.x + moveSpeed >= -MAX_SPEED && dirX == -1)
                {
                    rb.velocity = new Vector2(rb.velocity.x + moveSpeed * dirX, rb.velocity.y);
                }
                else
                {
                    rb.velocity = new Vector2(MAX_SPEED * dirX, rb.velocity.y);
                }

            }
            else
            { // SHIFT

                if (rb.velocity.x + moveSpeed <= MAX_SPEED * runMultiplier && dirX == 1)
                {
                    rb.velocity = new Vector2(rb.velocity.x + moveSpeed * dirX, rb.velocity.y);
                }
                else if (rb.velocity.x + moveSpeed >= -MAX_SPEED * runMultiplier && dirX == -1)
                {
                    rb.velocity = new Vector2(rb.velocity.x + moveSpeed * dirX, rb.velocity.y);
                }
                else
                {
                    rb.velocity = new Vector2(MAX_SPEED * runMultiplier * dirX, rb.velocity.y);
                }
            }
        }
        else
        { // IN AIR
            if (rb.velocity.x + moveSpeed > MAX_SPEED && dirX == 1)
            {
                rb.velocity = new Vector2(MAX_SPEED * dirX, rb.velocity.y);
            }
            else if (rb.velocity.x + moveSpeed < -MAX_SPEED && dirX == -1)
            {
                rb.velocity = new Vector2(MAX_SPEED * dirX, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x + moveSpeed * dirX, rb.velocity.y);
            }
        }

    }

    public void KeyDetection()
    {
        if (Input.GetButton("Jump") && !Key_Jump) Key_Jump = true;
        else if (!Input.GetButton("Jump") && Key_Jump) Key_Jump = false;
    }

    public bool GroundDetection()
    {
        if (Physics2D.BoxCast(coll[0].bounds.center, coll[0].bounds.size, 0f, Vector2.down, 0f, jumpableGround))
        {
            wallSideJumped_history = 0;
            if (Input.GetButton("Run"))
            {
                if (moveSpeed != InitialMoveSpeed * runMultiplier) moveSpeed = InitialMoveSpeed * runMultiplier;
            }
            else
            {
                if (moveSpeed != InitialMoveSpeed) moveSpeed = InitialMoveSpeed;
            }
            PlayerInAir = false;
            //GroundFriction();
            return true;
        }
        else
        {
            if (PlayerInAir)
            {
                if (moveSpeed * 1.05f <= InitialMoveSpeed)
                {
                    moveSpeed = moveSpeed * 1.05f;
                }
                else
                {
                    moveSpeed = InitialMoveSpeed;
                }
            }
            else
            { // On Jump
                PlayerInAir = true;
                moveSpeed = InitialMoveSpeed / 3;
            }

            return false;
        }
    }

    public bool GroundFriction()
    {
        if (PlayerInAir) return false;
        rb.velocity = new Vector2(rb.velocity.x * groundFriction, rb.velocity.y);
        return true;
    }


    public int WallDetection()
    {
        int wallStatus = GetWallCollision();

        if (wallStatus == 0)
        {
            isWallLocked = false;
            if (rb.gravityScale != GravityForce) rb.gravityScale = GravityForce;
            return 0;
        }

        int wallSide = GetWallCollision();

        if (Key_Jump && wallSide != wallSideJumped_history)
        { // Wall Boost

            wallSideJumped_history = wallSide;

            if (wallSide == 1) wallSide = 1;
            else if (wallSide == 2) wallSide = -1;
            else Debug.Log("ERRO 986235 - Not touching any wall (Unable to Wall-Jump)");
            rb.velocity = new Vector2(wallBoost / 2 * wallSide, wallBoost);

            if (wallSide == -1) rb.rotation -= ROTATION_RATE_DEFAULT * Time.deltaTime;
            else if (wallSide == 1) rb.rotation += ROTATION_RATE_DEFAULT * Time.deltaTime;

            return 2;
        }
        else
        { // In Ground

            if (!Input.GetButton("holdOnWall"))
            {
                if (rb.gravityScale != GravityForce) rb.gravityScale = GravityForce;
                if (rb.velocity.y < -WallSlideSpeed) rb.velocity = new Vector2(rb.velocity.x, -WallSlideSpeed);
                isWallLocked = false;
            }
            else
            { //SHIFT
                isWallLocked = true;
                if (rb.gravityScale != 0f) rb.gravityScale = 0f;
                rb.velocity = new Vector2(rb.velocity.x, 0f);
            }
            return 1;
        }

    }

    private int GetWallCollision()
    {
        if (Physics2D.BoxCast(coll[1].bounds.center, coll[1].bounds.size, 0f, Vector2.left, 0.0f, jumpableGround))
        {
            return 1;
        }
        else if (Physics2D.BoxCast(coll[2].bounds.center, coll[2].bounds.size, 0f, Vector2.right, 0.0f, jumpableGround))
        {
            return 2;
        }
        else
        {
            return 0;
        }
    }


    private void UpdateAnimationState()
    {
        MovementState state;
        //Debug.Log(GetFramesUntilCollision(false));
        if (GetWallCollision() == 1)
        {
            state = MovementState.OnWall;
            anim.SetBool(holdOnWallParameter, true);
        }
        else if (GetWallCollision() == 2)
        {
            state = MovementState.OnWall;
            anim.SetBool(holdOnWallParameter, true);
            sprite.flipX = rb.velocity.x < 0f;
        }
        else if (rb.velocity.y > 0.1f || rb.velocity.y < 0.1f && !isNearGround()) 
        {
            state = MovementState.Jump;
            sprite.flipX = rb.velocity.x < 0f;
            anim.SetBool(holdOnWallParameter, false);
        }
        else if (rb.velocity.y < -0.1f && isNearGround())
        {
            state = MovementState.Falling;
            sprite.flipX = rb.velocity.x < 0f;
            anim.SetBool(holdOnWallParameter, false);
        }
        else if (dirX != 0f)
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

    private int PlayerAutoRotate()
    {
        float player_rotation = rb.rotation;
        //Debug.Log(player_rotation);
        if (player_rotation >= 360 || player_rotation <= -360)
        {
            rb.rotation = 0f;
            return 3;
        }
        else if (player_rotation == 0f) return 0;
        else if (player_rotation > 0f)
        {
            rb.rotation += ROTATION_RATE_DEFAULT;
            return 1;
        }
        else if (player_rotation < 0f)
        {
            rb.rotation -= ROTATION_RATE_DEFAULT;
            return 2;
        }
        else
        {
            return 0;
        }

    }

    private int GetFramesUntilCollision(bool show)
    {


        float velocityX = rb.velocity.x;
        float velocityY = rb.velocity.y;

        float posX = rb.position.x;
        float posY = rb.position.y;

        if (show)
        {

            foreach (Transform child in CollisionDetectorGroup)
            {
                Destroy(child.gameObject);
            }

        }

        for (int i = 0; i < 100; i++)
        {
            float newposX = posX + velocityX * (i * 0.1f);
            float newposY = posY + (velocityY * (i * 0.1f)) - (GravityForce * i * i * 0.1f);

            Vector2 newColliderPosition = new Vector2(newposX, newposY);

            //displayX = coll[3].bounds.center.x - PosX;
            //displayY = coll[3].bounds.center.y - PosY;            

            if (show)
            {
                GameObject newSpriteObject = new GameObject("CollisionDetector_" + i);
                newSpriteObject.transform.SetParent(CollisionDetectorGroup);
                SpriteRenderer spriteRenderer = newSpriteObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = CollisionDetectorSprite;
                newSpriteObject.transform.position = newColliderPosition;
                newSpriteObject.transform.localScale = new Vector2(0.3f, 0.3f);
            }

            if (Physics2D.BoxCast(newColliderPosition, coll[3].bounds.size, 0f, Vector2.right, 0.0f, jumpableGround))
            {
                return i;
            }
        }

        return 0;

    }
    private bool isNearGround()
    {
        return (Physics2D.BoxCast(coll[0].bounds.center, coll[0].bounds.size, 0f, Vector2.down, 3f, jumpableGround));
    }

}
