using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPitch : MonoBehaviour
{
    AudioSource sound;

    private void Awake()
    {
        sound = gameObject.GetComponent<AudioSource>();
        sound.pitch = Random.Range(0.8f,1.2f);
    }
}
