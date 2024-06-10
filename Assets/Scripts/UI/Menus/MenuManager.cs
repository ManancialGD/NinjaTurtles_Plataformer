using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.Audio;

public class MenuManager : MonoBehaviour
{
    SaveOptionsSystem saveSystem;
    SceneManage sceneManage;
    ChangeLanguageDash dashLanguage;
    LanguageManager languageManager;

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
        dashLanguage = FindObjectOfType<ChangeLanguageDash>();
        languageManager = FindObjectOfType<LanguageManager>();

        if (sceneManage.CurrentScene == "LevelSelect")
        {

        }
        else
        {
            DoubleClickDash = true;

            saveSystem = GetComponent<SaveOptionsSystem>();
            int i = saveSystem.LoadDashData();
            if (i == 0) ChangeDashType();
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
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
        sceneManage.FreezeGame(true);
    }

    public void ContinueGame()
    {
        PausePanel.SetActive(false);
        GamePaused = false;
        menuState = MenuState.Playing;
        sceneManage.FreezeGame(false);
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
            if (languageManager.language == 1)
            {
                TextMeshProUGUI dashTypeText = DashTypeTextObject.GetComponent<TextMeshProUGUI>();
                dashTypeText.text = dashLanguage.english2;
            }
            else 
            {
                TextMeshProUGUI dashTypeText = DashTypeTextObject.GetComponent<TextMeshProUGUI>();
                dashTypeText.text = dashLanguage.portuguese2;
            }

            saveSystem.SaveDashType(0);
        }
        else
        {
            DoubleClickDash = true;
            if (languageManager.language == 1)
            {
                TextMeshProUGUI dashTypeText = DashTypeTextObject.GetComponent<TextMeshProUGUI>();
                dashTypeText.text = dashLanguage.english;
            }
            else 
            {
                TextMeshProUGUI dashTypeText = DashTypeTextObject.GetComponent<TextMeshProUGUI>();
                dashTypeText.text = dashLanguage.portuguese;
            }
            saveSystem.SaveDashType(1);
        }
    }

}
