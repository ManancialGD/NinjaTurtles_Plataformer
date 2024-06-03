using UnityEngine;
using UnityEngine.UI;

public class LeoAnimation : MonoBehaviour
{
    LeoCollisionDetector coll;
    private Animator anim;
    private LeoAttacks leoAttacks;
    private LeoStats leoStats;
    private string currentAnimation;

    private const float movementThreshold = 0.5f; // Threshold for significant movement
    private float movementX;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        leoAttacks = GetComponent<LeoAttacks>();
        leoStats = GetComponent<LeoStats>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<LeoCollisionDetector>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckAnimation();
    }

    private void CheckAnimation()
    {
        if (rb.velocity.y > .01f && coll.isNearGround && !leoAttacks.isAttacking)
        {
            ChangeAnimation("LeoJump");
        }
        else if ((rb.velocity.y > .01f || rb.velocity.y < -.01f) && !coll.isNearGround && !leoAttacks.isAttacking)
        {
            ChangeAnimation("LeoAirRoll");
        }
        else if (rb.velocity.y < -.01f && !leoAttacks.isAttacking)
        {
            ChangeAnimation("LeoFall");
        }
        else if (Mathf.Abs(rb.velocity.x) > movementThreshold && !leoAttacks.isAttacking)
        {
            if (leoStats.InStaminaBreak)  ChangeAnimation("LeoWalk");
            else ChangeAnimation("LeoRun");
        }
        else if (Mathf.Abs(rb.velocity.x) < 0.1f && !leoAttacks.isAttacking) // If velocity.x is close to zero
        {
            ChangeAnimation("LeoIdle");
        }

    }

    public void ChangeAnimation(string newAnimation)
    {
        if (currentAnimation != newAnimation)
        {
            currentAnimation = newAnimation;
            anim.Play(newAnimation);
        }
    }

}
