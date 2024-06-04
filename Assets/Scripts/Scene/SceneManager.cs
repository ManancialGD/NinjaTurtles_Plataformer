using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    Transform leo;

    CameraFollow cameraFollow;
    PauseMenu pauseMenu;
    public bool GamePaused { get; private set; }

    private void Awake()
    {
        leo = FindObjectOfType<LeoMovement>().transform;
        cameraFollow = FindObjectOfType<CameraFollow>();
        pauseMenu = FindObjectOfType<PauseMenu>(true);
        ChangeCameraTarget(leo);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            if (!GamePaused)
            {
                pauseMenu.Pause();
            }
            else if (GamePaused)
            {
                pauseMenu.Continue();
            }
        }
    }

    // This is a method for the future, now we set the target as the player in the awake
    // But in the future we can change this to another thing
    private void ChangeCameraTarget(Transform target)
    {
        cameraFollow.SetTarget(target);
    }

    public void SetGamePaused (bool b)
    {
        GamePaused = b;
    }
}
