using UnityEngine;

/// <summary>
/// Class to handle enemy movement towards Leo and detect ground collision.
/// </summary>
public class ShooterMovement : MonoBehaviour
{
    private Transform leo;
    private Rigidbody2D rb;
    private DetectLeo leoDetection;

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 2f;
    private float defaultMovementSpeed;

    [Header("Ground Detection")]
    [SerializeField] private GameObject forwardGroundCheckPosition; // Position to check for ground collision when moving forward
    [SerializeField] private GameObject backwardGroundCheckPosition; // Position to check for ground collision when moving backward
    [SerializeField] private float groundCheckRadius = 0.5f; // Radius for the ground check
    [SerializeField] private bool isGroundedForward;
    [SerializeField] private bool isGroundedBackward;


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
        if (forwardGroundCheckPosition != null)
        {
            isGroundedForward = Physics2D.OverlapCircle(forwardGroundCheckPosition.transform.position, groundCheckRadius, groundLayers);
        }

        if (backwardGroundCheckPosition != null)
        {
            isGroundedBackward = Physics2D.OverlapCircle(backwardGroundCheckPosition.transform.position, groundCheckRadius, groundLayers);
        }
    }

    /// <summary>
    /// Handles the movement of the enemy based on Leo's position.
    /// Moves towards Leo if grounded and away if Leo is detected in the area.
    /// </summary>
    private void HandleMovement()
    {
        if (!isGroundedForward && !isGroundedBackward) return;
        
        float direction = leo.position.x < transform.position.x ? -1 : 1;
        
        if (leoDetection.IsLeoInArea)
        {
            if (isGroundedBackward)
            {
                direction *= -1;
                rb.velocity = new Vector2(direction * movementSpeed, rb.velocity.y);
            }      
        }
        else
        {
            if (isGroundedForward)
            {
                rb.velocity = new Vector2(direction * movementSpeed, rb.velocity.y);

            }
        }
    }

    /// <summary>
    /// Draws gizmos in the editor for visualization.
    /// Draws a wire sphere for the detection radius and a circle for the ground check.
    /// </summary>
    private void OnDrawGizmos()
    {
        // Draw the ground check circles
        if (forwardGroundCheckPosition != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(forwardGroundCheckPosition.transform.position, groundCheckRadius);
        }

        if (backwardGroundCheckPosition != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(backwardGroundCheckPosition.transform.position, groundCheckRadius);
        }
    }
}
