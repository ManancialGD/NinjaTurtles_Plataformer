using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{
    [Header("Layers")]
    public LayerMask jumpableGround;

    [Space]

    public bool onGround;
    public bool onWall;
    public bool onRightWall;
    public bool onLeftWall;
    public int wallSide;

    [Space]

    [Header("Collision")]
    public Vector2 wallBoxSize = new Vector2(0.04f, 0.61f);
    public Vector2 groundBoxSize = new Vector2(0.5f, 0.1f);
    public Vector2 groundOffset, rightWallOffset, leftWallOffset;
    Color debugCollisionColor = Color.red;

    // Update is called once per frame
    void Update()
    {
        onGround = Physics2D.OverlapBox((Vector2)transform.position + groundOffset, groundBoxSize, 0f, jumpableGround);
        onWall = Physics2D.OverlapBox((Vector2)transform.position + leftWallOffset, wallBoxSize, 0f, jumpableGround)
            || Physics2D.OverlapBox((Vector2)transform.position + rightWallOffset, wallBoxSize, 0f, jumpableGround);

        onRightWall = Physics2D.OverlapBox((Vector2)transform.position + rightWallOffset, wallBoxSize, 0f, jumpableGround);
        onLeftWall = Physics2D.OverlapBox((Vector2)transform.position + leftWallOffset, wallBoxSize, 0f, jumpableGround);

        wallSide = onRightWall ? -1 : 1;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = debugCollisionColor;

        Gizmos.DrawWireCube((Vector2)transform.position + groundOffset, groundBoxSize); // Ground Collision
        Gizmos.DrawWireCube((Vector2)transform.position + leftWallOffset, wallBoxSize); // Left wall Collision
        Gizmos.DrawWireCube((Vector2)transform.position + rightWallOffset, wallBoxSize); // Right wall Collision
    }
}
