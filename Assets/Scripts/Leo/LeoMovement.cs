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


        private float defaultGravityScale;
        private Vector2 playerInput;
        private bool alreadyFlipped;

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
            
            if (Input.GetButtonDown("Jump") && coll.onGround) 
            {
                leoVelocity.y = jumpSpeed;
                rb.gravityScale = 1.0f;
                jumpTime = Time.time; // This will prevent to fall slowly whitout jumping first

            }
            else if (Input.GetButton("Jump") && ((Time.time - jumpTime) < maxJumpTime)) // If is holding jump, then fall slowly
            {
                rb.gravityScale = 1.0f;
            }
            else
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

            leoVelocity.x = playerInput.x *  movementeSpeed; // if the input is negative, player will go to the left

            // Flip Leo acording to the movement
            if (playerInput.x < -0.01f && !alreadyFlipped) Flip(true);
            else if (playerInput.x > 0.01f && alreadyFlipped) Flip(false);

            rb.velocity = leoVelocity;
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
