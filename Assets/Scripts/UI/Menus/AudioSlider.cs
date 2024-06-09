using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSlider : MonoBehaviour
{   
    SaveOptionsSystem saveOptionsSystem;
    [SerializeField] Slider slider;
    [SerializeField] AudioMixer mixer;

    private void Awake()
    {
        saveOptionsSystem = FindObjectOfType<SaveOptionsSystem>();
        slider.value = saveOptionsSystem.LoadAudioVolumeData();
        ChangeAudio();
    }

    public void ChangeAudio()
    {
        mixer.SetFloat("MasterVolume", slider.value);
        saveOptionsSystem.SaveAudioVolume(slider.value);
    }
}
