using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeoMovement : MonoBehaviour
{
    LeoCollisionDetector coll;
    Rigidbody2D rb;
    PlayerInputs playerInputs;

    [Header("Colliders")]
    [SerializeField] Collider2D groundCollider;
    [SerializeField] Collider2D airCollider;

    [Space]

    [Header("Speed")]
    [SerializeField] private int movementeSpeed = 150;
    [SerializeField] private int jumpSpeed = 200;
    [SerializeField] private Vector2 wallJumpSpeed = new Vector2(150, 200);
    private int defaultMovementSpeed;
    private int defaultJumpSpeed;

    [Space]

    [Header("Times")]

    [SerializeField] private float jumpTime;
    [SerializeField] private float maxJumpTime = 0.4f;

    [Space]

    [Header("Bools")]
    [SerializeField] private bool canMove = true;
    public bool CanMove { get { return canMove; } private set { canMove = value; } }

    [SerializeField] private bool canJump = true;
    public bool CanJump { get { return canJump; } private set { canJump = value; } }

    public bool IsFacingRight { get; private set; }
    [SerializeField] public bool Sliding { get; private set; }
    [SerializeField] private bool wallJumped;

    private bool alreadyFlipped;


    private float defaultGravityScale;
    private Vector2 playerInput;


    void Awake()
    {
        defaultJumpSpeed = jumpSpeed;
        defaultMovementSpeed = movementeSpeed;

        IsFacingRight = true; // Is facing right start as true

        playerInputs = GetComponent<PlayerInputs>();

        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<LeoCollisionDetector>();

        defaultGravityScale = rb.gravityScale;
    }


    void Update()
    {
        // Get player Inputs
        playerInput = playerInputs.input;

        // Change collisions if is in air
        airCollider.enabled = !coll.onGround;
        groundCollider.enabled = coll.onGround;

        // Here the slide collision is kinda off, you can see that his hands are not on the wall sometimes.
        // We could fix this by putting the collider only in his hand on the top, but I don't know if
        // this is a good thing or not.
        if (coll.onWall && !coll.onGround)
        {
            if (rb.velocity.y < -0.1f)
            {
                rb.gravityScale = .1f;
                Sliding = true;
            }
        }
        else if (coll.onGround || wallJumped || !coll.onWall)
        {
            rb.gravityScale = defaultGravityScale;
            Sliding = false;
        }
        if (coll.onWall || coll.onGround)
        {
            wallJumped = false;
        }
        if (canJump) Jump(); // Jump Logics
    }

    private void FixedUpdate()
    {
        if (CanMove) Move();
    }

    /// <summary>
    /// This method will make the leo jump higher and fall slower if the player is holding the Jump button
    /// This piece of code was made by professor Diogo
    /// </summary>
    private void Jump()
    {
        Vector3 leoVelocity = rb.velocity;

        if (Input.GetButtonDown("Jump"))
        {
            if (coll.onGround && !Sliding)
            {
                leoVelocity.y = jumpSpeed;
                rb.gravityScale = 1.0f;
                jumpTime = Time.time; // This will prevent to fall slowly whitout jumping first
            }
            else if (Sliding)
            {
                WallJump();
            }
        }
        else if ((Input.GetButton("Jump") && ((Time.time - jumpTime) < maxJumpTime)) && !Sliding ) // If is holding jump, then fall slowly
        {
            rb.gravityScale = 1.0f;
        }
        else if (!Sliding)
        {
            rb.gravityScale = defaultGravityScale;
        }

        rb.velocity = leoVelocity;
    }

    /// <summary>
    /// Make the player Move
    /// This doesn't have acceleration
    /// </summary>
    private void Move()
    {
        Vector3 leoVelocity = rb.velocity;

        leoVelocity.x = playerInput.x * movementeSpeed; // if the input is negative, player will go to the left

        // Flip Leo acording to the movement
        if (playerInput.x < -0.01f && !alreadyFlipped) Flip(true);
        else if (playerInput.x > 0.01f && alreadyFlipped) Flip(false);

        rb.velocity = leoVelocity;
    }
    private void WallJump()
    {
        int wallSide = 0;
        if (coll.onLeftWall) wallSide = 1;
        else if (coll.onRightWall) wallSide = -1;

        Vector3 tmpWallJumpVelocity = rb.velocity;
        tmpWallJumpVelocity = new Vector3(wallJumpSpeed.x * wallSide, wallJumpSpeed.y, 0);
        rb.velocity = tmpWallJumpVelocity;
        wallJumped = true;
    }

    // Rotate 180 degrees in the y axis
    private void Flip(bool flip)
    {
        if (flip)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            IsFacingRight = false;
            alreadyFlipped = true;
        }
        else if (!flip)
        {
            transform.rotation = Quaternion.identity;
            IsFacingRight = true;
            alreadyFlipped = false;
        }
    }

    // Getters and Setters
    public void SetCanMove(bool b)
    {
        canMove = b;
    }

    public void SetCanJump(bool b)
    {
        canJump = b;
    }

    public void StaminaBroke()
    {
        movementeSpeed = 50;
        jumpSpeed = 50;
    }
    public void StaminaRecovered()
    {
        movementeSpeed = defaultMovementSpeed;
        jumpSpeed = defaultJumpSpeed;
    }
}
