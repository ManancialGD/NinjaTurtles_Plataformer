using System;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    public int language;

    // Define a delegate for the language changed event
    public delegate void LanguageChangedHandler();
    // Define an event based on the delegate
    public event LanguageChangedHandler OnLanguageChanged;

    // Method to trigger the event
    public void TriggerEvent()
    {
        Debug.Log("Triggering language change event.");
        if (OnLanguageChanged != null)
        {
            OnLanguageChanged();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeLanguage(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeLanguage(0);
        }
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
