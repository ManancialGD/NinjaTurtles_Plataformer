using UnityEngine;
using TMPro;
using UnityEditor;

public class MenuManager : MonoBehaviour
{
    SaveOptionsSystem saveSystem;
    SceneManage sceneManage;

    [SerializeField] private GameObject PausePanel;
    [SerializeField] private GameObject OptionsPanel;
    [SerializeField] private GameObject DashTypeTextObject;  // GameObject
    [SerializeField] private GameObject DashButtonTextObject;  // GameObject

    [SerializeField] private GameObject LevelSelectionPanel;
    [SerializeField] private GameObject PilotLevelPanel;

    public bool GamePaused { get; private set; }

    public bool DoubleClickDash { get; private set; }

    private enum MenuState { Playing, Pause, Settings }
    private MenuState menuState = MenuState.Playing;
    private void Awake()
    {
        saveSystem = GetComponent<SaveOptionsSystem>();
        sceneManage = FindObjectOfType<SceneManage>();


        if (sceneManage.CurrentScene == "MainMenu" || sceneManage.CurrentScene == "Test") 
        {
            DoubleClickDash = true;

            int i = saveSystem.LoadData();
            if (i == 0) ChangeDashType();
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            if (sceneManage.CurrentScene == "Test")
            {
                if (menuState == MenuState.Settings)
                {
                    ReturnSetting();
                }
                else if (menuState == MenuState.Pause)
                {
                    ContinueGame();
                }
                else
                {
                    PauseGame();
                }

            }
            else if (sceneManage.CurrentScene == "MainMenu")
            {
                if (menuState == MenuState.Settings)
                {
                    ReturnSetting();
                }
            }
            else if (sceneManage.CurrentScene == "LevelSelect")
            {
                if (menuState == MenuState.Settings)
                {
                    ReturnLevelSelection();
                    Debug.Log("A");
                }
                else if (menuState == MenuState.Playing)
                {
                    sceneManage.ChangeScene("MainMenu");
                }
            }
        }
    }

    public void PauseGame()
    {
        PausePanel.SetActive(true);
        OptionsPanel.SetActive(false);  // Hide options panel if it's open
        GamePaused = true;
        menuState = MenuState.Pause;
        Time.timeScale = 0;
    }

    public void ContinueGame()
    {
        PausePanel.SetActive(false);
        GamePaused = false;
        menuState = MenuState.Playing;
        Time.timeScale = 1;
    }

    public void SettingMenu()
    {
        OptionsPanel.SetActive(true);
        PausePanel.SetActive(false);
        menuState = MenuState.Settings;
    }

    public void ReturnSetting()
    {
        OptionsPanel.SetActive(false);
        PausePanel.SetActive(true);
        menuState = MenuState.Pause;
    }

    public void ReturnLevelSelection()
    {
        LevelSelectionPanel.SetActive(true);
        PilotLevelPanel.SetActive(false);
        menuState = MenuState.Playing;
    }
    public void PilotLevel()
    {
        PilotLevelPanel.SetActive(true);
        LevelSelectionPanel.SetActive(false);
        menuState = MenuState.Settings;
    }

    public void ChangeDashType()
    {
        TextMeshProUGUI dashTypeText = DashTypeTextObject.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI dashButtonText = DashButtonTextObject.GetComponent<TextMeshProUGUI>();

        if (DoubleClickDash)
        {
            DoubleClickDash = false;
            dashTypeText.text = "Dash2 USE SHIFT: ";
            dashButtonText.text = "";
            saveSystem.SaveDashType(0);
        }
        else
        {
            DoubleClickDash = true;
            dashTypeText.text = "Dash1 DOUBLE CLICK: ";
            dashButtonText.text = "O";
            saveSystem.SaveDashType(1);
        }
    }
}
