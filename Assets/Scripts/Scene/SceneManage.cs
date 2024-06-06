using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneManage : MonoBehaviour
{
    Transform leo;
    CameraFollow cameraFollow;
    MenuManager menuManager;
    public string CurrentScene { get; private set; }
    private void Awake()
    {

        // Create a temporary reference to the current scene.
        Scene CurrentSceneScene = SceneManager.GetActiveScene();

        // Retrieve the name of this scene.
        CurrentScene = CurrentSceneScene.name;

        menuManager = FindObjectOfType<MenuManager>();

        if (CurrentScene == "MainMenu")
        {
            
        }
        else if (CurrentScene == "Test")
        {
            leo = FindObjectOfType<LeoMovement>().transform;
            cameraFollow = FindObjectOfType<CameraFollow>();
            ChangeCameraTarget(leo);
        }
        else if (CurrentScene == "PilotLevel")
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
        CurrentScene = scene;
        menuManager.ContinueGame();
    }
    public void Quit()
    {
        Application.Quit();
    }
}
