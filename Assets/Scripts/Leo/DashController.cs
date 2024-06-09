using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashController : MonoBehaviour
{
    Rigidbody2D rb;
    LeoMovement leoMovement;
    LeoStats leoStats;
    LeoAttacks leoAttacks;
    LeoCollisionDetector leoColls;
    MenuManager menuManager;
    LeoAudio leoAudio;

    [SerializeField] float dashForce = 500;
    [SerializeField] float dashCooldown = 0.3f;
    int timesPressed = 0;
    char lastSidePressed = 'I';
    float detectionTime = 0.2f;
    [SerializeField] bool canDash = false;
    [SerializeField] bool hasExtraDash;
    [SerializeField] bool onCooldown;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        leoMovement = GetComponent<LeoMovement>();
        leoAttacks = GetComponent<LeoAttacks>();
        leoColls = GetComponent<LeoCollisionDetector>();
        leoStats = GetComponent<LeoStats>();
        menuManager = FindObjectOfType<MenuManager>();
        leoAudio = FindObjectOfType<LeoAudio>();
    }

    void Update()
    {
        if (leoStats.InStaminaBreak) return;

        if (leoAttacks.isAttacking) return;
        if (menuManager.DoubleClickDash) Dash1();
        else Dash2();
    }

    private void Dash1()
    {
        if (detectionTime > 0 && timesPressed > 0) detectionTime -= Time.deltaTime;
        if (detectionTime < 0)
        {
            timesPressed = 0;
            detectionTime = 0.2f;
        }

        if (leoColls.onGround && !canDash || leoColls.onWall && !canDash) canDash = true;

        if (!canDash) return;

        Vector2 dashVelocity = new Vector2(0, 0);

        char currentSidePressed = 'I';
        if (Input.GetKeyDown(KeyCode.D))
        {
            currentSidePressed = 'D';
            if (detectionTime >= 0) timesPressed++;
            detectionTime = 0.2f;
            dashVelocity = new Vector2(dashForce, 0);
            if (lastSidePressed != 'D') timesPressed = 1;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            currentSidePressed = 'A';
            if (detectionTime >= 0) timesPressed++;
            detectionTime = 0.2f;
            dashVelocity = new Vector2(-dashForce, 0);
            if (lastSidePressed != 'A') timesPressed = 1;
        }

        if (currentSidePressed != 'I' && timesPressed >= 2)
        {
            print(lastSidePressed + " - " + currentSidePressed);
            if (lastSidePressed == currentSidePressed)
            {
                timesPressed = 0;
                detectionTime = 2f;
                lastSidePressed = 'I';
                StartCoroutine(Dash(dashVelocity, dashCooldown));
                return;
            }
            else timesPressed = 1;
        }
        if (currentSidePressed != 'I') lastSidePressed = currentSidePressed;
    }

    private void Dash2()
    {
        if (onCooldown)
        {
            canDash = false;
        }
        else if (leoColls.onGround || leoColls.onWall)
        {
            hasExtraDash = true;
            canDash = true;
        }
        else if (!leoColls.onGround && !leoColls.onWall && hasExtraDash)
        {
            if (Input.GetButtonDown("Dash"))
            {
                hasExtraDash = false;
                canDash = true;
            }
        }
        else
        {
            canDash = false;
        }

        if (!canDash) return;

        if (Input.GetButtonDown("Dash"))
        {
            Vector2 dashVelocity;
            if (leoMovement.IsFacingRight)
            {
                dashVelocity = new Vector2 (dashForce, 0);
            }
            else dashVelocity = new Vector2 (-dashForce, 0);
            
            StartCoroutine(Dash(dashVelocity, dashCooldown));
        }
    }

    IEnumerator Dash(Vector2 velocity, float cooldown)
    {
        leoStats.isInvulnerable = true;
        leoAudio.PlayDashSound();
        canDash = false;
        rb.velocity = new Vector2 (velocity.x, rb.velocity.y);

        leoStats.ConsumeStamina(15);

        onCooldown = true;
        yield return new WaitForSeconds(.3f);

        rb.velocity = new Vector2(rb.velocity.x * 0.1f, rb.velocity.y) ;
        canDash = false;
        leoStats.isInvulnerable = false;
        yield return new WaitForSeconds(cooldown);
        
        onCooldown = false;
    }

}