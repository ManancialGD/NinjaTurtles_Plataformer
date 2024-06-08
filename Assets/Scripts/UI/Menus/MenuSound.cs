using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSound : MonoBehaviour
{
    [SerializeField] AudioSource clickSound;
    [SerializeField] AudioSource paperSound;

    public void PlayClickSound()
    {
        clickSound.pitch = Random.Range(0.8f, 1.2f);
        clickSound.Play();
    }
    
    public void PlayPaperSound()
    {
        paperSound.pitch = Random.Range(0.8f, 1.2f);
        paperSound.Play();
    }
}
