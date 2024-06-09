using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectAndDamageLeo : MonoBehaviour
{
    DetectLeo leodetection;
    private bool canDamage;

    private void Awake()
    {
        leodetection = GetComponentInParent<DetectLeo>();
        canDamage = true;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        LeoStats leoHP = other.GetComponent<LeoStats>();
        if (leoHP != null)
        {
            if (canDamage)
            {
                if (leodetection.IsFacingRight) leoHP.TakeDamage(20, 1, new Vector2(150, 0));
                else leoHP.TakeDamage(20, 1, new Vector2(-150, 0));
                StartCoroutine(WaitToDamageAgain());
            }
        }
    }

    IEnumerator WaitToDamageAgain()
    {
        canDamage = false;
        yield return new WaitForSeconds(0.1f);
        canDamage = true;
    }
}
