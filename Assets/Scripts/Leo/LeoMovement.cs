using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LeoMovement : MonoBehaviour
{
    LeoCollisionDetector coll;
    Rigidbody2D rb;
    PlayerInputs playerInputs;
    MenuManager menuManager;
    LeoAudio leoAudio;

    [Header("Colliders")]
    [SerializeField] Collider2D groundCollider;
    [SerializeField] Collider2D airCollider;

    [Space]

    [Header("Speed")]
    [SerializeField] private float movementSpeed = 150f;
    [SerializeField] private float jumpSpeed = 200f;
    [SerializeField] private Vector2 wallJumpSpeed = new Vector2(150f, 200f);
    private float defaultMovementSpeed;
    private float defaultJumpSpeed;
    private Vector2 defaultWallJumpSpeed;
    private float currentSpeed = 0f;
    private float speedIncrement = 75f; // Speed increment value

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
    [SerializeField] private bool jumped;
    private bool alreadyPlayingStep;

    private bool alreadyFlipped;
    private float defaultGravityScale;
    private Vector2 playerInput;

    void Awake()
    {
        defaultJumpSpeed = jumpSpeed;
        defaultMovementSpeed = movementSpeed;
        defaultWallJumpSpeed = wallJumpSpeed;

        IsFacingRight = true; // Start facing right
        playerInputs = GetComponent<PlayerInputs>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<LeoCollisionDetector>();
        defaultGravityScale = rb.gravityScale;
        menuManager = FindObjectOfType<MenuManager>();

        leoAudio = FindObjectOfType<LeoAudio>();
        jumped = false;
    }

    void Update()
    {
        if (menuManager.GamePaused) return;

        // Get player Inputs
        playerInput = playerInputs.input;
        // Change collisions if in air
        airCollider.enabled = !coll.onGround;
        groundCollider.enabled = coll.onGround;

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
        
        if (Sliding)
        {
            if (coll.onRightWall && !IsFacingRight)
            {
                Flip(true);
            }
            else if (coll.onLeftWall && IsFacingRight)
            {
                Flip(false);
            }
        }
        if (jumped && coll.onGround)
        {
            jumped = false;
            leoAudio.PlayLandSound();
        }

        if (canJump) Jump(); // Jump logic
    }
    private IEnumerator WaitBeforeJumped()
    {
        yield return new WaitForSeconds(0.1f);
        jumped = true;
    }
    private void FixedUpdate()
    {
        if (menuManager.GamePaused) return;
        if (CanMove) Move();
    }
    private void Jump()
    {
        Vector3 leoVelocity = rb.velocity;

        if (Input.GetButtonDown("Jump"))
        {
            if (coll.onGround && !Sliding)
            {
                leoVelocity.y = jumpSpeed;
                rb.gravityScale = 1.0f;
                jumpTime = Time.time; // Prevent falling slowly without jumping first
                
                StartCoroutine(WaitBeforeJumped());

                leoAudio.PlayJumpSound();
            }
            else if (Sliding)
            {
                WallJump();
                return; // Exit method to prevent overriding wall jump velocity
            }
        }
        else if ((Input.GetButton("Jump") && ((Time.time - jumpTime) < maxJumpTime)) && !Sliding) // Hold jump for higher jump
        {
            rb.gravityScale = 1.0f;
        }
        else if (!Sliding)
        {
            rb.gravityScale = defaultGravityScale;
        }

        rb.velocity = leoVelocity;
    }
    private void Move()
    {
        Vector3 leoVelocity = rb.velocity;

        // Incrementally add speed to current velocity until reaching the maximum speed
        if (playerInput.x != 0)
        {
            float targetSpeed = playerInput.x * movementSpeed;

            // Only change velocity if it's less than the movement speed or greater than -movement speed
            if ((leoVelocity.x < movementSpeed && playerInput.x > 0) || (leoVelocity.x > -movementSpeed && playerInput.x < 0))
            {
                leoVelocity.x = Mathf.MoveTowards(leoVelocity.x, targetSpeed, speedIncrement);
            }
        }

        // Flip character based on movement direction
        if (playerInput.x < -0.01f && !alreadyFlipped) Flip(true);
        else if (playerInput.x > 0.01f && alreadyFlipped) Flip(false);

        rb.velocity = leoVelocity;
    }

    private void WallJump()
    {
        int wallSide = coll.onRightWall ? -1 : 1;
        Vector2 wallJumpDirection = new Vector2(wallJumpSpeed.x * wallSide, wallJumpSpeed.y);
        rb.velocity = wallJumpDirection;
        wallJumped = true;

        if (IsFacingRight) Flip(true);
        else if (!IsFacingRight) Flip(false);

        leoAudio.PlayJumpSound();
        
        StartCoroutine(DisableMovement(0.2f)); // Prevent immediate re-sliding
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

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
        movementSpeed = 50f;
        jumpSpeed = 50f;
        wallJumpSpeed = new Vector2(50f, 50f);
    }
    public void StaminaRecovered()
    {
        movementSpeed = defaultMovementSpeed;
        jumpSpeed = defaultJumpSpeed;
        wallJumpSpeed = defaultWallJumpSpeed;
    }
}
