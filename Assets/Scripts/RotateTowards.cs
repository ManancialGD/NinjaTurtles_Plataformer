using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowards : MonoBehaviour
{
    Player playerScript; // Reference to the Player script
    public float rotationSpeed;
    public float timeToFix;
    public GameObject thisEnemy;
    Enemy_Ranger rangerScript;

    private void Start()
    {
        playerScript = FindObjectOfType<Player>();
        rangerScript = GetComponentInParent<Enemy_Ranger>();

        if (playerScript == null)
        {
            Debug.LogError("Script do Player não encontrado.");
        }
    }

    private void FixedUpdate()
    {
        if (playerScript != null)
        {
            Vector3 vectorToTarget = playerScript.transform.position - transform.position;
            float angle;


            if (rangerScript.attackCooldown > timeToFix)
            {
                if (!rangerScript.flipped) angle = -90f;
                else angle = 90f;
            }
            else
            {

                if (!rangerScript.flipped) angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - 180;
                else angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;

            }

            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * rotationSpeed);

        }
    }
}
