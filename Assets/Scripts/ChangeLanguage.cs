using TMPro;
using UnityEngine;

public class ChangeLanguage : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;  // Corrected the component name
    [SerializeField] private string english;
    [SerializeField] private string portuguese;  // Corrected the spelling

    public LanguageManager languageManager;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();  // Corrected the component name
        languageManager = FindObjectOfType<LanguageManager>();
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
