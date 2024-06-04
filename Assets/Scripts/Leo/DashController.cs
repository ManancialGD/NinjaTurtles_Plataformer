using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;

public class DashController : MonoBehaviour
{
    [SerializeField] float dashForce = 150;
    [SerializeField] float dashTime = 0.3f;
    int timesPressed = 0;
    char lastSidePressed = 'I';
    float detectionTime = 0.2f;
    bool canDash = false;
    Rigidbody2D rb;
    LeoMovement leoMovement;
    LeoCollisionDetector leoColls;
    MenuManager menuManager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        leoMovement = GetComponent<LeoMovement>();
        leoColls = GetComponent<LeoCollisionDetector>();
        menuManager = FindObjectOfType<MenuManager>();
    }

    void Update()
    {
        if (menuManager.GamePaused)return;

    	DoubleClickDash();
    }

    private void DoubleClickDash()
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

    IEnumerator Dash(Vector2 velocity, float time, char side)
    {
        canDash = false;
        rb.velocity = velocity;

        //float simulatedTime = 0;

        yield return new WaitForSeconds(0.1f);

        rb.velocity *= 0.1f;
        canDash = false;
    }
}
