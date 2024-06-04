using UnityEngine;
using TMPro;

public class MenuManager : MonoBehaviour
{
    SaveOptionsSystem saveSystem;
    SceneManage sceneManage;

    [SerializeField] private GameObject PausePanel;
    [SerializeField] private GameObject OptionsPanel;
    [SerializeField] private GameObject DashTypeTextObject;  // GameObject
    [SerializeField] private GameObject DashButtonTextObject;  // GameObject
    public bool GamePaused { get; private set; }

    public bool DoubleClickDash { get; private set; }

    private enum MenuState { Playing, Pause, Settings }
    private MenuState menuState = MenuState.Playing;
    private void Awake()
    {
        saveSystem = GetComponent<SaveOptionsSystem>();
        sceneManage = FindObjectOfType<SceneManage>();

        DoubleClickDash = true;

        int i = saveSystem.LoadData();
        if (i == 0) ChangeDashType();
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

    public void ChangeDashType()
    {
        TextMeshProUGUI dashTypeText = DashTypeTextObject.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI dashButtonText = DashButtonTextObject.GetComponent<TextMeshProUGUI>();

        if (DoubleClickDash)
        {
            DoubleClickDash = false;
            dashTypeText.text = "Shift to dash: ";
            dashButtonText.text = "";
            saveSystem.SaveDashType(0);
        }
        else
        {
            DoubleClickDash = true;
            dashTypeText.text = "Double click dash: ";
            dashButtonText.text = "O";
            saveSystem.SaveDashType(1);
        }
    }
}
