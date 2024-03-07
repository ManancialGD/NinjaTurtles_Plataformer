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
    public float wallCollisionRadius = 0.25f;
    public float groundCollisionRadius = 0.25f;
    public Vector2 bottomOffset, rightOffset, leftOffset;
    Color debugCollisionColor = Color.red;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        onGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, groundCollisionRadius, jumpableGround);
        onWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, wallCollisionRadius, jumpableGround)
            || Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, wallCollisionRadius, jumpableGround);

        onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, wallCollisionRadius, jumpableGround);
        onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, wallCollisionRadius, jumpableGround);

        wallSide = onRightWall ? -1 : 1;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = debugCollisionColor;

        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, groundCollisionRadius); // Ground Collision
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, wallCollisionRadius); // Left wall Collision
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, wallCollisionRadius); // Right wall Collision
    }
}
