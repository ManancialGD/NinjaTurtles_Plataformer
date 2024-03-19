using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootParticles : MonoBehaviour
{
    // Start is called before the first frame update

    Player playerScript;

    void Start()
    {
        playerScript = FindObjectOfType<Player>();
        RotateTowardsPlayer();
    }

    void RotateTowardsPlayer()
    {
        Vector3 vectorToTarget = playerScript.transform.position - transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - 180;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = q;
    }


}
