using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneManage : MonoBehaviour
{
    Transform leo;
    CameraFollow cameraFollow;
    public string currentScene;
    private void Awake()
    {

        // Create a temporary reference to the current scene.
        Scene currentSceneScene = SceneManager.GetActiveScene();

        // Retrieve the name of this scene.
        currentScene = currentSceneScene.name;

        if (currentScene == "MainMenu")
        {

        }
        else
        {
            leo = FindObjectOfType<LeoMovement>().transform;
            cameraFollow = FindObjectOfType<CameraFollow>();
            ChangeCameraTarget(leo);
        }

    }

    // This is a method for the future, now we set the target as the player in the awake
    // But in the future we can change this to another thing
    private void ChangeCameraTarget(Transform target)
    {
        cameraFollow.SetTarget(target);
    }

    public void ChangeScene(string scene)
    {
        SceneManager.LoadScene(scene);
        currentScene = scene;
    }
    public void Quit()
    {
        Application.Quit();
    }
}
