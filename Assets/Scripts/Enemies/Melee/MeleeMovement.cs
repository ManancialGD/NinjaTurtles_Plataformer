using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeMovement : MonoBehaviour
{
    private Transform leo;
    private Rigidbody2D rb;
    private DetectLeo leoDetection;
    MeleeAttack attack;
    EnemyHP hp;

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 2f;
    private float defaultMovementSpeed;

    [Header("Ground Detection")]
    [SerializeField] private GameObject GroundCheckPosition; // Position to check for ground collision
    [SerializeField] private float groundCheckRadius = 0.5f; // Radius for the ground check
    [SerializeField] private bool isGroundedForward;


    [Header("Layers")]
    [SerializeField] private LayerMask groundLayers;


    /// <summary>
    /// Initialize components needed for this class.
    /// </summary>
    private void Awake()
    {
        leoDetection = GetComponent<DetectLeo>();
        leo = FindObjectOfType<LeoMovement>().transform;
        rb = GetComponent<Rigidbody2D>();
        hp = GetComponent<EnemyHP>();
        attack = GetComponent<MeleeAttack>();

        defaultMovementSpeed = movementSpeed;
    }

    /// <summary>
    /// Update is called once per frame.
    /// Detects if Leo is in the area, checks ground collision, and handles movement towards or away from Leo.
    /// </summary>
    private void Update()
    {
        if (!leoDetection.leoDetected) return;
        CheckGroundCollision();
        HandleMovement();
    }

    /// <summary>
    /// Checks if there is ground collision at the specified ground check positions.
    /// </summary>
    private void CheckGroundCollision()
    {
        if (GroundCheckPosition != null)
        {
            isGroundedForward = Physics2D.OverlapCircle(GroundCheckPosition.transform.position, groundCheckRadius, groundLayers);
        }
    }

    /// <summary>
    /// Handles the movement of the enemy based on Leo's position.
    /// Moves towards Leo if grounded and away if Leo is detected in the area.
    /// </summary>
    private void HandleMovement()
    {
        if (!isGroundedForward) return;
        if (hp.IsStunned) return;
        if (attack.isAttacking) return;

        float direction = leo.position.x < transform.position.x ? -1 : 1;

        if (isGroundedForward)
        {
            rb.velocity = new Vector2(direction * movementSpeed, rb.velocity.y);

        }
    }

    /// <summary>
    /// Draws gizmos in the editor for visualization.
    /// Draws a wire sphere for the detection radius and a circle for the ground check.
    /// </summary>
    private void OnDrawGizmos()
    {
        // Draw the ground check circles
        if (GroundCheckPosition != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(GroundCheckPosition.transform.position, groundCheckRadius);
        }
    }
}
