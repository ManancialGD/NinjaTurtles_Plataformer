using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    Transform leo;

    CameraFollow cameraFollow;

    private void Awake()
    {
        leo = FindObjectOfType<LeoMovement>().transform;
        cameraFollow = FindObjectOfType<CameraFollow>();

        ChangeCameraTarget(leo);
    }

    // This is a method for the future, now we set the target as the player in the awake
    // But in the future we can change this to another thing
    private void ChangeCameraTarget(Transform target)
    {
        cameraFollow.SetTarget(target);
    }
}
