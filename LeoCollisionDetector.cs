using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class will check some colliders
/// <summary>
public class LeoCollisionDetector : MonoBehaviour
{
    [Header("Layers")]
    public LayerMask jumpableGround;
    public LayerMask jumpableWalls;

    [Space]

    [Header("Bools")]

    public bool onGround;
    public bool onWall;
    public bool onRightWall;
    public bool onLeftWall;
    public bool isNearGround;

    [Space]

    [Header("Wall Collisions")]
    public Vector2 wallBoxSize = new Vector2(0.04f, 0.61f);
    public Vector2 rightWallOffset, leftWallOffset;

    [Space]

    public Color debugWallCollisionColor = Color.blue;

    [Space]

    [Header("Ground Collisions")]
    public Vector2 groundBoxSize = new Vector2(0.5f, 0.1f);
    public Vector2 groundOffset;
    [Space]
    public Vector2 nearGroundBoxSize = new Vector2(0.5f, 50f);
    public Vector2 nearGroundOffset = new Vector2(0.5f, 50f);

    [Space]

    public Color debugGroundCollisionColor = Color.red;

    void Update()
    {
        CheckCollision();
    }

    /// <summary>
    /// Will create a box that if has something overlaping, will make the bool true.
    /// <summary>
    private void CheckCollision()
    {
        // Ground Checkers

        onGround = Physics2D.OverlapBox((Vector2)transform.position + groundOffset, groundBoxSize, 0f, jumpableGround);
        isNearGround = Physics2D.OverlapBox((Vector2)transform.position + nearGroundOffset, nearGroundBoxSize, 0f, jumpableGround);

        // Wall Checkers 

        onRightWall = Physics2D.OverlapBox((Vector2)transform.position + rightWallOffset, wallBoxSize, 0f, jumpableWalls);
        onLeftWall = Physics2D.OverlapBox((Vector2)transform.position + leftWallOffset, wallBoxSize, 0f, jumpableWalls);

        // onWall if when is touching any wall, can be useful in some cases
        if (onRightWall || onLeftWall) onWall = true;
        else onWall = false;
    }
    
    private void OnDrawGizmos()
    {
        // Draw wall colliders
        Gizmos.color = debugWallCollisionColor;

        Gizmos.DrawWireCube((Vector2)transform.position + leftWallOffset, wallBoxSize);
        Gizmos.DrawWireCube((Vector2)transform.position + rightWallOffset, wallBoxSize);

        // Draw ground colliders
        Gizmos.color = debugGroundCollisionColor;

        Gizmos.DrawWireCube((Vector2)transform.position + nearGroundOffset, nearGroundBoxSize);
        Gizmos.DrawWireCube((Vector2)transform.position + groundOffset, groundBoxSize);
    }
}
