using System.Collections;
using UnityEngine;

public class EnemyAim : MonoBehaviour
{

    public GameObject prefabToSpawn;
    public GameObject prefabParticles;
    ParticleSystem shootParticles;

    void Start()
    {
        shootParticles = GetComponentInChildren<ParticleSystem>();
    }

    public void ShootBullet()
    {

        shootParticles.Play();

        Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
    }
}
