using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeoAudio : MonoBehaviour
{   
    [SerializeField] private AudioSource jumpSound;
    [SerializeField] private AudioSource landSound;
    [SerializeField] private AudioSource stepSound;
    [SerializeField] private AudioSource attackSound;
    [SerializeField] private AudioSource dashSound;

    public void PlayJumpSound()
    {
        jumpSound.pitch = Random.Range(0.8f, 1.2f);
        jumpSound.Play();
    }
    public void PlayLandSound()
    {
        landSound.pitch = Random.Range(0.8f, 1.2f);
        landSound.Play();
    }

    public void PlayStepSound()
    {
        stepSound.pitch = Random.Range(0.8f, 1.2f);
        stepSound.Play();
    }

    public void PlayAttackSound()
    {
        attackSound.pitch = Random.Range(0.8f, 1.2f);
        attackSound.Play();
    }
    public void PlayDashSound()
    {
        dashSound.pitch = Random.Range(0.8f, 1.2f);
        dashSound.Play();
    }
}
