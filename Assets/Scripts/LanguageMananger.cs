using System;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    public int language;

    // Define a delegate for the language changed event
    public delegate void LanguageChangedHandler();
    // Define an event based on the delegate
    public event LanguageChangedHandler OnLanguageChanged;
    DataSystem dataSystem;

    // Method to trigger the event
    public void TriggerEvent()
    {
        Debug.Log("Triggering language change event.");
        if (OnLanguageChanged != null)
        {
            OnLanguageChanged();
        }
    }

    void Awake()
    {
        dataSystem = FindObjectOfType<DataSystem>();
        GameData gameData = dataSystem.LoadData();

        if (gameData.Language == "English") language = 0;
        else language = 1;
        ChangeLanguage(language);
    }

    // Method to change the language
    public void ChangeLanguage(int i)
    {
        if (i != 0 && i != 1) return;
        language = i;
        Debug.Log("Language changed to: " + language);
        TriggerEvent();
    }

    // Method to get the current language
    public int GetLanguage()
    {
        return language;
    }
}
