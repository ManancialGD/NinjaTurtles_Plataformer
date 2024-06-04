using System.Collections;
using UnityEngine;

public class DashController : MonoBehaviour
{

    Rigidbody2D rb;
    LeoMovement leoMovement;
    LeoAttacks leoAttacks;
    LeoStats leoStats;
    LeoCollisionDetector coll;
    MenuManager menuManager;

    [Header("Stats")]
    [SerializeField] float dashForce = 150f;
    [SerializeField] float dashTime = 0.3f;
    [SerializeField] float dashCooldown = 0.5f;
    [SerializeField] float airDashCooldown = 0.5f;
    [SerializeField] float airDashForceMultiplier = 0.5f;

    int timesPressed = 0;
    char lastSidePressed = 'I';
    float detectionTime = 0.2f;

    [Space]

    [Header("Bools")]
    [SerializeField] bool canDash = true;
    [SerializeField] bool wasTouchingSurface = true;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        leoMovement = GetComponent<LeoMovement>();
        leoAttacks = GetComponent<LeoAttacks>();
        leoStats = GetComponent<LeoStats>();
        coll = GetComponent<LeoCollisionDetector>();
        menuManager = FindObjectOfType<MenuManager>();
    }

    void Update()
    {
        if (leoStats.Stamina < 25) return;
        if (leoStats.InStaminaBreak) return;
        if (menuManager.GamePaused) return;

        // Check if the player is grounded or touching a wall
        if ((coll.onGround || coll.onWall) && !wasTouchingSurface)
        {
            wasTouchingSurface = true;
            canDash = true;
        }
        else if (!coll.onGround && !coll.onWall)
        {
            wasTouchingSurface = false;
        }

        // Check if the player can dash
        if (leoAttacks.isAttacking || !canDash) return;

        if (menuManager.DoubleClickDash)
        {
            DoubleClickDash();
        }
        else
        {
            ShiftDash();
        }
    }

    private void DoubleClickDash()
    {
        if (detectionTime > 0 && timesPressed > 0) detectionTime -= Time.deltaTime;
        if (detectionTime < 0)
        {
            timesPressed = 0;
            detectionTime = 0.2f;
        }

        Vector2 dashVelocity = Vector2.zero;

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
            if (lastSidePressed == currentSidePressed)
            {
                timesPressed = 0;
                detectionTime = 2f;
                lastSidePressed = 'I';
                StartCoroutine(Dash(dashVelocity, dashTime, currentSidePressed));
                return;
            }
            else timesPressed = 1;
        }
        if (currentSidePressed != 'I') lastSidePressed = currentSidePressed;
    }

    private void ShiftDash()
    {
        if (Input.GetButtonDown("Dash"))
        {
            Vector2 dashVelocity;
            if (leoMovement.IsFacingRight)
            {
                dashVelocity = new Vector2(dashForce, 0);
            }
            else
            {
                dashVelocity = new Vector2(-dashForce, 0);
            }

            if (!coll.onGround) // Apply air dash cooldown and force multiplier if not grounded
            {
                StartCoroutine(Dash(dashVelocity * airDashForceMultiplier, dashTime, leoMovement.IsFacingRight ? 'D' : 'A', airDashCooldown));
            }
            else
            {
                StartCoroutine(Dash(dashVelocity, dashTime));
            }
        }
    }

    IEnumerator Dash(Vector2 velocity, float time, char side = 'N', float cooldown = 0f)
    {
        canDash = false;

        float startTime = Time.time;
        Vector2 initialVelocity = rb.velocity;

        while (Time.time < startTime + time)
        {
            float elapsedTime = Time.time - startTime;
            float dashProgress = Mathf.Clamp01(elapsedTime / time);

            Vector2 newVelocity = new Vector2(
                Mathf.Lerp(initialVelocity.x, velocity.x, dashProgress),
                rb.velocity.y
            );
            rb.velocity = newVelocity;

            yield return null;
        }

        rb.velocity = velocity;

        leoStats.ConsumeStamina(25);

        if (leoMovement.CanMove) leoMovement.SetCanMove(false);

        yield return new WaitForSeconds(0.1f);
        if (!leoMovement.CanMove) leoMovement.SetCanMove(true);

        if (cooldown > 0)
        {
            yield return new WaitForSeconds(cooldown);
        }

        canDash = true;
    }
}
