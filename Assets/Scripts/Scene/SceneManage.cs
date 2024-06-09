using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

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
        FreezeGame(false);
        StartCoroutine(WaitAndChangeScene(scene));
    }
    public void Quit()
    {
        Application.Quit();
    }

    public void FreezeGame(bool b)
    {
        if (b) Time.timeScale = 0;
        else Time.timeScale = 1;
    }

    IEnumerator WaitAndChangeScene(string scene)
    {
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene(scene);
        CurrentScene = scene;
        menuManager.ContinueGame();
    }
}
