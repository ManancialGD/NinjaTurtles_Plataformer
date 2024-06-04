using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SaveOptionsSystem : MonoBehaviour
{
    MenuManager menuManager;
    private void Awake()
    {
        menuManager = GetComponent<MenuManager>();
    }
    public void SaveDashType(int OnOff)
    {
        PlayerPrefs.SetInt("DoubleClickDash", OnOff);
    }

    public int LoadData()
    {
        return PlayerPrefs.GetInt("DoubleClickDash");
    }
}
