using System.Collections;
using UnityEngine;

public class EnemyAim : MonoBehaviour
{

    GameObject prefabToSpawn;
    public GameObject prefabParticles;
    ParticleSystem shootParticles;

    void Start()
    {
        shootParticles = GetComponentInChildren<ParticleSystem>();
    }

    public void ShootBullet()
    {
        Debug.Log("");
        if (ComputeVelocity(transform.position, target.position, prefabToSpawn.speed, Physics2D.gravity.y, minimizeTime, out Vector2 vel))
        {
            var newShot = Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
            newShot.SetVelocity(vel);
        }
        else
        {
            Debug.LogWarning("Impossible to hit target!");
        }
    }
}
