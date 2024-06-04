using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject PausePanel;

    private SceneManager sceneManager;
    void Awake()
    {
        sceneManager = FindObjectOfType<SceneManager>();
    }

    public void Pause()
    {
        PausePanel.SetActive(true);
        sceneManager.SetGamePaused(true);
        Time.timeScale = 0;
    }

    public void Continue()
    {
        PausePanel.SetActive(false);
        sceneManager.SetGamePaused(false);
        Time.timeScale = 1;
    }
}
