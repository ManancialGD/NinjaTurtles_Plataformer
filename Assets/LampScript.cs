using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampScript : MonoBehaviour
{
    
    GameObject thisLamp;
    Player playerScript;
    NativeInfo native;
    BoxCollider2D boxCollider2D;
    float angular_velocity = 0f;
    Rigidbody2D rb;

    void Start()
    {
        thisLamp = GetComponent<GameObject>();
        rb = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        rb.angularVelocity = angular_velocity;
    }

    public void SetLampAngularVelocity(float newAngularVelocity){

        angular_velocity = newAngularVelocity;
        return;

    }
}
