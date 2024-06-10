using TMPro;
using UnityEngine;

public class ChangeLanguageDash : MonoBehaviour
{
    MenuManager menuManager;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] public string english;
    [SerializeField] public string english2;
    [SerializeField] public string portuguese;
    [SerializeField] public string portuguese2;

    public LanguageManager languageManager;

    private void Awake()
    {
        menuManager = FindObjectOfType<MenuManager>();
        languageManager = FindObjectOfType<LanguageManager>();
        if (menuManager == null)
        {
            Debug.LogError("MenuManager component not found!");
        }

        text = GetComponent<TextMeshProUGUI>();  // Corrected the component name
        if (text == null)
        {
            Debug.LogError("TextMeshProUGUI component not found!");
        }
    }

    private void Start()
    {
        if (languageManager != null)
        {
            // Subscribe to the event
            languageManager.OnLanguageChanged += RespondToLanguageChange;
            // Update the text initially based on the current language
            RespondToLanguageChange();
        }
        else
        {
            Debug.LogError("LanguageManager reference is not set!");
        }
    }

    // Method to respond to the language change event
    public void RespondToLanguageChange()
    {
        if (languageManager == null || menuManager == null)
        {
            Debug.LogError("RespondToLanguageChange called with null references!");
            return;
        }

        if (languageManager.GetLanguage() == 0)
        {
            if (menuManager.DoubleClickDash)
            {
                text.text = english;
            }
            else
            {
                text.text = english2;
            }
        }
        else if (languageManager.GetLanguage() == 1)
        {
            if (menuManager.DoubleClickDash)
            {
                text.text = portuguese;
            }
            else
            {
                text.text = portuguese2;
            }
        }
    }

    private void OnDestroy()
    {
        if (languageManager != null)
        {
            // Unsubscribe from the event
            languageManager.OnLanguageChanged -= RespondToLanguageChange;
        }
    }
}
