using UnityEngine;

/// <summary>
/// Class to detect Leo and handle rotation towards Leo.
/// </summary>
public class DetectLeo : MonoBehaviour
{
    private Transform leo;

    [Header("Bools")]
    [SerializeField] private bool leoOnRight;
    [SerializeField] public bool IsFacingRight;
    [SerializeField] public bool IsFacingLeo;
    [SerializeField] public bool IsLeoInArea;
    [SerializeField] public bool leoDetected;

    [Space]

    [Header("DetectCollider")]
    [SerializeField] private float collisionRadius = 80f;

    [Header("Layers")]
    [SerializeField] private LayerMask groundLayers;

    private bool canSeeLeo;

    /// <summary>
    /// Initialize stuff that this class will need
    /// </summary>
    private void Awake()
    {
        leo = FindObjectOfType<LeoMovement>().transform;
    }

    /// <summary>
    /// Update is called once per frame.
    /// Detects if Leo is in the area and handles rotation towards Leo.
    /// </summary>
    private void Update()
    {
        DetectLeoInArea();
        if (canSeeLeo) RotateTowardsLeo();
    }

    /// <summary>
    /// Detects if Leo is within the specified collision radius and if there is a clear line of sight.
    /// Updates the isFacingLeo and leoDetected flags accordingly.
    /// </summary>
    private void DetectLeoInArea()
    {
        // Check if Leo is within the collision radius
        Collider2D hitCollider = Physics2D.OverlapCircle(transform.position, collisionRadius, LayerMask.GetMask("Leo"));
        IsLeoInArea = hitCollider != null;

        // Determine if the object is facing Leo based on current direction and Leo's position
        if ((IsFacingRight && leoOnRight) || (!IsFacingRight && !leoOnRight))
        {
            IsFacingLeo = true;
        }
        else
        {
            IsFacingLeo = false;
        }

        // Check if there's a clear line of sight to Leo if the object is facing Leo
        if (IsFacingLeo || IsLeoInArea)
        {
            canSeeLeo = CanSeeLeo();
            leoDetected = canSeeLeo;
        }
        else
        {
            leoDetected = false;
        }
    }

    /// <summary>
    /// Checks if there is a clear line of sight to Leo by performing a linecast.
    /// </summary>
    /// <returns>True if there is a clear line of sight to Leo, otherwise false.</returns>
    private bool CanSeeLeo()
    {
        RaycastHit2D hit = Physics2D.Linecast(transform.position, leo.position, groundLayers);
        return hit.collider == null;
    }

    /// <summary>
    /// Rotates the object to face towards Leo based on Leo's position.
    /// Only rotates if there is a clear line of sight to Leo.
    /// </summary>
    private void RotateTowardsLeo()
    {
        if (leo.position.x < transform.position.x)
        {
            leoOnRight = false;
            if ((IsFacingRight && IsFacingLeo) || IsLeoInArea)
            {
                if (canSeeLeo) Rotate(true);
            }
        }
        else if (leo.position.x > transform.position.x)
        {
            leoOnRight = true;
            if ((!IsFacingRight && IsFacingLeo) || IsLeoInArea)
            {
                if (canSeeLeo) Rotate(false);
            }
        }
    }

    /// <summary>
    /// Rotates the object based on the specified direction.
    /// </summary>
    /// <param name="b">True to rotate to the left, false to rotate to the right.</param>
    private void Rotate(bool b)
    {
        if (b)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            IsFacingRight = false;
        }
        else
        {
            transform.rotation = Quaternion.identity;
            IsFacingRight = true;
        }
    }

    /// <summary>
    /// Draws gizmos in the editor for visualization.
    /// Draws a wire sphere for the detection radius and a line indicating line of sight to Leo.
    /// </summary>
    private void OnDrawGizmos()
    {
        // Draw detection radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, collisionRadius);

        // Draw the line indicating line of sight only if the object is facing Leo
        if (leo != null && IsFacingLeo)
        {
            Gizmos.color = canSeeLeo ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position, leo.position);
        }
    }
}