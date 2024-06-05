using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashController : MonoBehaviour
{
    Rigidbody2D rb;
    LeoMovement leoMovement;
    LeoStats leoStats;
    LeoCollisionDetector leoColls;
    MenuManager menuManager;

    [SerializeField] float dashForce = 200;
    [SerializeField] float dashTime = 0.3f;
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
        leoColls = GetComponent<LeoCollisionDetector>();
        leoStats = GetComponent<LeoStats>();
        menuManager = FindObjectOfType<MenuManager>();
    }

    void Update()
    {
        if (leoStats.InStaminaBreak) return;

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
            print(lastSidePressed);
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
                StartCoroutine(Dash1(dashVelocity, dashTime, currentSidePressed));
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
            
            StartCoroutine(Dash2(dashVelocity,  .4f));
        }
    }

    IEnumerator Dash1(Vector2 velocity, float time, char side)
    {
        canDash = false;
        rb.velocity = velocity;

        //float simulatedTime = 0;
        leoStats.ConsumeStamina(15);
        
        yield return new WaitForSeconds(0.1f);

        rb.velocity *= 0.1f;
        canDash = false;
    }
    IEnumerator Dash2(Vector2 velocity, float cooldown)
    {
        onCooldown = true;
        rb.velocity = new Vector2(rb.velocity.x + velocity.x, rb.velocity.y);

        leoStats.ConsumeStamina(15);

        yield return new WaitForSeconds(cooldown);

        onCooldown = false;
    }
}