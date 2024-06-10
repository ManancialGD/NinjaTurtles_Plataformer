using TMPro;
using UnityEngine;

public class ChangeLanguageDash : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] public string english;
    [SerializeField] public string english2;
    [SerializeField] public string portuguese;
    [SerializeField] public string portuguese2;

    public LanguageManager languageManager;

    private void Awake()
    {
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
    private void RespondToLanguageChange()
    {
        Debug.Log("Language has been changed to: " + languageManager.GetLanguage());
        if (languageManager.GetLanguage() == 0)
        {
            text.text = english;
        }
        else if (languageManager.GetLanguage() == 1)
        {
            text.text = portuguese;
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
