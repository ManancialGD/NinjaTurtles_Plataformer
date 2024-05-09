using UnityEngine;
using UnityEngine.UI;

public class LeoAnimation : MonoBehaviour
{
    private Animator anim;
    private Player player;
    private string currentAnimation;
    public bool isFacingRight { get; private set; }
    private const float movementThreshold = 0.5f; // Threshold for significant movement
    private float movementX;
    private Rigidbody2D rb;
    public bool isAttacking = false;

    // Start is called before the first frame update
    void Start()
    {
        isFacingRight = true;
        anim = GetComponent<Animator>();
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        movementX = Input.GetAxis("Horizontal");
        CheckAnimation();
        if (Input.GetButtonDown("Attack"))
        {
            ChangeAnimation("LeoSlashAttack");
        }



        if (rb.velocity.x < -.1f && !isAttacking) FlipAnimation(true);
        else if (rb.velocity.x > .1f && !isAttacking) FlipAnimation(false);
        
    }

    private void CheckAnimation()
    {
        if (Mathf.Abs(movementX) > movementThreshold && !isAttacking)
        {
            ChangeAnimation("LeoRun");
        }
        else if (Mathf.Abs(rb.velocity.x) < 0.1f && !isAttacking) // If velocity.x is close to zero
        {
            ChangeAnimation("LeoIdle");
        }
        else if (!isAttacking)
        {
            ChangeAnimation("LeoWalk"); // Play the walk animation when moving slowly
        }
    }

    private void ChangeAnimation(string newAnimation)
    {
        if (currentAnimation != newAnimation)
        {
            currentAnimation = newAnimation;
            anim.Play(newAnimation);
        }
    }

    private void FlipAnimation(bool flip)
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
}
