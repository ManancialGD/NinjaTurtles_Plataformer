using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAudio: MonoBehaviour
{
    [SerializeField] private AudioSource swordHit;
    [SerializeField] private AudioSource shootSound;

    public void PlaySwordHitSound()
    {
        swordHit.pitch = Random.Range(0.8f, 1.2f);
        swordHit.Play();
    }

    public void PlayShootSound()
    {
        shootSound.pitch = Random.Range(0.8f, 1.2f);
        shootSound.Play();
    }
}
