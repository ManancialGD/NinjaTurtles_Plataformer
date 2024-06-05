using UnityEngine;

/// <summary>
/// Class to handle enemy movement towards Leo and detect ground collision.
/// </summary>
public class ShooterMovement : MonoBehaviour
{
    private Transform leo;
    private Rigidbody2D rb;

    public bool leoDetected { get; private set; }

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 2f;

    [Header("Ground Detection")]
    [SerializeField] private GameObject groundCheckPosition; // Position to check for ground collision
    [SerializeField] private float groundCheckRadius = 0.5f; // Radius for the ground check

    [Header("Detection")]
    [SerializeField] private float detectionRadius = 5f; // Radius for detecting Leo
    [SerializeField] private LayerMask leoLayer;

    [Header("Layers")]
    [SerializeField] private LayerMask groundLayers;

    private bool isGrounded;

    /// <summary>
    /// Initialize stuff that this class will need.
    /// </summary>
    private void Awake()
    {
        leo = FindObjectOfType<LeoMovement>().transform;
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Update is called once per frame.
    /// Detects if Leo is in the area, checks ground collision, and handles movement towards or away from Leo.
    /// </summary>
    private void Update()
    {
        CheckGroundCollision();
        DetectLeo();
        HandleMovement();
    }

    /// <summary>
    /// Checks if there is ground collision at the specified ground check position.
    /// </summary>
    private void CheckGroundCollision()
    {
        if (groundCheckPosition != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheckPosition.transform.position, groundCheckRadius, groundLayers);
        }
    }

    /// <summary>
    /// Detects if Leo is within the specified detection radius.
    /// </summary>
    private void DetectLeo()
    {
        Collider2D hitCollider = Physics2D.OverlapCircle(transform.position, detectionRadius, leoLayer);
        leoDetected = hitCollider != null;
    }

    /// <summary>
    /// Handles the movement of the enemy based on Leo's position.
    /// Moves towards Leo if grounded and away if Leo is detected in the area.
    /// </summary>
    private void HandleMovement()
    {
        if (!isGrounded) return;

        float direction = leoDetected ? -1 : 1; // Move away if Leo is detected, otherwise move towards

        if (leo.position.x < transform.position.x)
        {
            direction *= -1;
        }

        rb.velocity = new Vector2(direction * movementSpeed, rb.velocity.y);

        // Flip the sprite based on movement direction
        if (direction > 0 && transform.localScale.x < 0 || direction < 0 && transform.localScale.x > 0)
        {
            Flip();
        }
    }

    /// <summary>
    /// Flips the character's sprite.
    /// </summary>
    private void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    /// <summary>
    /// Draws gizmos in the editor for visualization.
    /// Draws a wire sphere for the detection radius and a circle for the ground check.
    /// </summary>
    private void OnDrawGizmos()
    {
        // Draw the ground check circle
        if (groundCheckPosition != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(groundCheckPosition.transform.position, groundCheckRadius);
        }

        // Draw the detection radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
