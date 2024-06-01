    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class LeoMovement : MonoBehaviour
    {
        LeoCollisionDetector coll;
        Rigidbody2D rb;
        PlayerInputs playerInputs;

        public bool isFacingRight { get; private set; }

        [SerializeField] private int MovementeSpeed = 200;
        [SerializeField] private int jumpSpeed = 250;

        [SerializeField] private float jumpTime;
        [SerializeField] private float maxJumpTime = 0.1f;

        [SerializeField] private bool canMove = true;
        public bool CanMove { get { return canMove; } private set { canMove = value; } }

        [SerializeField] private bool canJump = true;
        public bool CanJump { get { return canJump; } private set { canJump = value; } }


        private float defaultGravityScale;
        private Vector2 playerInput;

        void Awake()
        {
            isFacingRight = true; // Is facing right start as true

            playerInputs = GetComponent<PlayerInputs>();

            rb = GetComponent<Rigidbody2D>();
            coll = GetComponent<LeoCollisionDetector>();

            defaultGravityScale = rb.gravityScale;
        }


        void Update()
        {
            // Get player Inputs
            playerInput = playerInputs.input;
            
            if (canJump) Jump(); // Jump Logics
        }

        private void FixedUpdate()
        {
            if (CanMove) Move();
        }

        /// <summary>
        /// This method will make the leo jump higher and fall slower if the player is holding the Jump button
        /// Code made by Professor Diogo
        /// </summary>
        private void Jump()
        {
            Vector3 leoVelocity = rb.velocity;   
            
            if (Input.GetButtonDown("Jump") && coll.onGround)
            {
                leoVelocity.y = jumpSpeed;
                rb.gravityScale = 1.0f;
                jumpTime = Time.time;

            }
            else if (Input.GetButton("Jump") && ((Time.time - jumpTime) < maxJumpTime))
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
        /// Made by Professor Diogo
        /// </summary>
        private void Move()
        {
            Vector3 leoVelocity = rb.velocity;

            leoVelocity.x = playerInput.x *  MovementeSpeed; // if the input is negative, player will go to the left

            // Flip Leo acording to the movement
            if (playerInput.x > -0.01f) Flip(true);
            else if (playerInput.x < 0.01f) Flip(false);

            rb.velocity = leoVelocity;
        }

        private void Flip(bool flip)
        {
            if (flip)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
                isFacingRight = false;
            }
            else if (!flip)
            {
                transform.rotation = Quaternion.identity;
                isFacingRight = true;
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
    }
