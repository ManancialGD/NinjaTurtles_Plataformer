using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LeoAnimation : MonoBehaviour
{
    private Animator anim;
    private Player player;
    private string currentAnimation;
    private bool isFacingRight = true; // Assuming character starts facing right
    private const float movementThreshold = 0.5f; // Threshold for significant movement
    private float movementX;
    private Rigidbody2D rb;
    public bool isAttacking = false;

    // Start is called before the first frame update
    void Start()
    {
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

        if (Input.GetAxisRaw("Horizontal") < -.01f && !isAttacking) FlipAnimation(false);
        else if (Input.GetAxisRaw("Horizontal") > .01f && !isAttacking) FlipAnimation(true);

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

    private void FlipAnimation(bool facingRight)
    {
        if (facingRight && !isFacingRight)
        {
            Debug.Log("FLIP - true");
            isFacingRight = true;
            Vector3 newScale = transform.localScale;
            newScale.x = 1;
            transform.localScale = newScale;
        }
        else if (!facingRight && isFacingRight)
        {
            Debug.Log("FLIP - false");
            isFacingRight = false;
            Vector3 newScale = transform.localScale;
            newScale.x = -1;
            transform.localScale = newScale;
        }
    }
}
