using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeoCollisionDetector : MonoBehaviour
{
    [Header("Layers")]
    public LayerMask jumpableGround;
    public LayerMask jumpableWalls;

    [Space]

    public bool onGround;
    public bool onWall;
    public bool onRightWall;
    public bool onLeftWall;
    public bool isNearGround;

    [Space]

    [Header("Collision")]
    public Vector2 wallBoxSize = new Vector2(0.04f, 0.61f);
    public Vector2 groundBoxSize = new Vector2(0.5f, 0.1f);
    public Vector2 groundOffset, rightWallOffset, leftWallOffset, attackCollision;
    public Vector2 nearGroundOffset = new Vector2(0.5f, 50f);
    public Vector2 nearGroundBoxSize = new Vector2(0.5f, 50f);
    public Color debugCollisionColor = Color.red;

    void Update()
    {
        onGround = Physics2D.OverlapBox((Vector2)transform.position + groundOffset, groundBoxSize, 0f, jumpableGround);
        isNearGround = Physics2D.OverlapBox((Vector2)transform.position + nearGroundOffset, nearGroundBoxSize, 0f, jumpableGround);
        onWall = Physics2D.OverlapBox((Vector2)transform.position + leftWallOffset, wallBoxSize, 0f, jumpableWalls)
            || Physics2D.OverlapBox((Vector2)transform.position + rightWallOffset, wallBoxSize, 0f, jumpableWalls);

        onRightWall = Physics2D.OverlapBox((Vector2)transform.position + rightWallOffset, wallBoxSize, 0f, jumpableWalls);
        onLeftWall = Physics2D.OverlapBox((Vector2)transform.position + leftWallOffset, wallBoxSize, 0f, jumpableWalls);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = debugCollisionColor;

        Gizmos.DrawWireCube((Vector2)transform.position + nearGroundOffset, nearGroundBoxSize);
        Gizmos.DrawWireCube((Vector2)transform.position + groundOffset, groundBoxSize);
        Gizmos.DrawWireCube((Vector2)transform.position + leftWallOffset, wallBoxSize);
        Gizmos.DrawWireCube((Vector2)transform.position + rightWallOffset, wallBoxSize);
    }
}
