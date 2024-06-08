using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LeoAnimation : MonoBehaviour
{
    LeoCollisionDetector coll;
    LeoAudio leoAudio;
    private bool alreadyPlayingStep;
    private Animator anim;
    private LeoAttacks leoAttacks;
    private LeoStats leoStats;
    private LeoMovement leoMov;
    private string currentAnimation;

    private const float movementThreshold = 0.5f; // Threshold for significant movement
    private float movementX;
    private Rigidbody2D rb;
    MenuManager menuManager;

    // Start is called before the first frame update
    void Start()
    {
        leoAttacks = GetComponent<LeoAttacks>();
        leoStats = GetComponent<LeoStats>();
        leoMov = GetComponent<LeoMovement>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<LeoCollisionDetector>();
        menuManager = FindObjectOfType<MenuManager>();
        leoAudio = FindObjectOfType<LeoAudio>();
        alreadyPlayingStep = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (menuManager.GamePaused) return;
        CheckAnimation();

        if (!alreadyPlayingStep)
        {
            if (currentAnimation == "LeoWalk")
            {
                StartCoroutine(PlayStepSound(.6f));
                leoAudio.PlayStepSound();
            }
            else if (currentAnimation == "LeoRun")
            {
                StartCoroutine(PlayStepSound(.4f));
                leoAudio.PlayStepSound();
            }
        }
    }
    private IEnumerator PlayStepSound(float time)
    {
        alreadyPlayingStep = true;

        yield return new WaitForSeconds(time);

        alreadyPlayingStep = false;
    }
    private void CheckAnimation()
    {
        if (leoMov.Sliding)
        {
            ChangeAnimation("LeoWallSlide");
        }
        else if (rb.velocity.y > .01f && coll.isNearGround && !leoAttacks.isAttacking)
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
            if (leoStats.InStaminaBreak) ChangeAnimation("LeoWalk");
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
