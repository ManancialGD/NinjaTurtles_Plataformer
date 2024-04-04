using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LampScript : MonoBehaviour
{
    bool playerAttached = false;
    CircleCollider2D physicalCollider;
    CircleCollider2D detectionCollider;

    [SerializeField] GameObject LampPrefabReference;

    NativeInfo native;
    void Start()
    {
        native = FindObjectOfType<NativeInfo>();
        physicalCollider = GetComponents<CircleCollider2D>()[0];
        detectionCollider = GetComponents<CircleCollider2D>()[1];
    }
    void Update()
    {
        if (playerAttached)
        {
            if (!Input.GetMouseButton(0))
            {
                playerAttached = false;
                return;
            }
            float distance = native.GetDistance(transform.position, native.GetPlayerObj(native.currentPlayerID).transform.position).Item2;

            if (distance > 3f) playerAttached = false;
            else
            {
                if (!physicalCollider.isTrigger) physicalCollider.isTrigger = true;
                native.GetPlayerObj(native.currentPlayerID).transform.position = gameObject.transform.position;
            }
        }
        else if (physicalCollider.isTrigger)
        {
            float distance = native.GetDistance(transform.position, native.GetPlayerObj(native.currentPlayerID).transform.position).Item2;
            if (distance < 1f) physicalCollider.isTrigger = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (Input.GetMouseButton(0) && other.gameObject.CompareTag("Player"))
        {
            playerAttached = true;
            Debug.Log("--- Attached ---");
        }
    }

    public bool isPlayerAttached() => playerAttached;

}
