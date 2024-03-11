using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowards : MonoBehaviour
{
    Player playerScript; // Reference to the Player script
    public float rotationSpeed = 5f;
    public float rotationModifier;

    private void Start()
    {
        playerScript = FindObjectOfType<Player>();

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
            float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - rotationModifier;
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * rotationSpeed);
        }
    }
}
