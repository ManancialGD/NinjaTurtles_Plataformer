using TMPro;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    SaveOptionsSystem saveSystem;
    SceneManage sceneManage;
    ChangeLanguageDash dashLanguage;
    LanguageManager languageManager;
    [SerializeField] TextMeshProUGUI dashTypeText;
    [SerializeField] private GameObject PausePanel;
    [SerializeField] private GameObject OptionsPanel;
    [SerializeField] private GameObject DashTypeTextObject;  // GameObject

    [SerializeField] private GameObject LevelSelectionPanel;
    [SerializeField] private GameObject PilotLevelPanel;

    public bool GamePaused { get; private set; }

    public bool DoubleClickDash { get; private set; }

    private enum MenuState { Playing, Pause, Settings }
    private MenuState menuState = MenuState.Playing;

    private void Awake()
    {
        sceneManage = FindObjectOfType<SceneManage>();
        if (sceneManage == null)
        {
            Debug.LogError("SceneManage component not found!");
        }

        dashLanguage = DashTypeTextObject.GetComponent<ChangeLanguageDash>();
        if (dashLanguage == null)
        {
            Debug.LogError("ChangeLanguageDash component not found!");
        }

        languageManager = FindObjectOfType<LanguageManager>();
        if (languageManager == null)
        {
            Debug.LogError("LanguageManager component not found!");
        }

        if (sceneManage != null && sceneManage.CurrentScene == "LevelSelect")
        {
            // Additional initialization for L  evelSelect scene
        }
        else
        {
            DoubleClickDash = true;
            saveSystem = GetComponent<SaveOptionsSystem>();
            if (saveSystem != null)
            {
                int i = saveSystem.LoadDashData();
                if (i == 0) ChangeDashType();
            }
            else
            {
                Debug.LogError("SaveOptionsSystem component not found!");
            }
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            if (sceneManage == null)
            {
                Debug.LogError("SceneManage component is null in Update method!");
                return;
            }

            if (sceneManage.CurrentScene == "MainMenu")
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
                }
                else if (menuState == MenuState.Playing)
                {
                    sceneManage.ChangeScene("MainMenu");
                }
            }
            else if (sceneManage.CurrentScene == "PilotLevel")
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
        }
    }

    public void PauseGame()
    {
        PausePanel.SetActive(true);
        OptionsPanel.SetActive(false);  // Hide options panel if it's open
        GamePaused = true;
        menuState = MenuState.Pause;
        if (sceneManage != null)
        {
            sceneManage.FreezeGame(true);
        }
    }

    public void ContinueGame()
    {
        PausePanel.SetActive(false);
        GamePaused = false;
        menuState = MenuState.Playing;
        if (sceneManage != null)
        {
            sceneManage.FreezeGame(false);
        }
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
        if (DoubleClickDash)
        {
            DoubleClickDash = false;
        }
        else
        {
            DoubleClickDash = true;
        }

        if (dashLanguage != null)
        {
            dashLanguage.RespondToLanguageChange();
        }
        else
        {
            Debug.LogError("ChangeLanguageDash component is null in ChangeDashType method!");
        }

        if (saveSystem != null)
        {
            saveSystem.SaveDashType(DoubleClickDash ? 1 : 0);
        }
    }
}
