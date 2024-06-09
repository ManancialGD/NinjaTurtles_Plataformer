using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SaveOptionsSystem : MonoBehaviour
{
    public void SaveDashType(int OnOff)
    {
        PlayerPrefs.SetInt("DoubleClickDash", OnOff);
    }

    public void SaveAudioVolume(float volume)
    {
        PlayerPrefs.SetFloat("volume", volume);
    }
    public int LoadDashData()
    {
        return PlayerPrefs.GetInt("DoubleClickDash");
    }
    public float LoadAudioVolumeData()
    {
        return PlayerPrefs.GetFloat("volume");
    }
}
