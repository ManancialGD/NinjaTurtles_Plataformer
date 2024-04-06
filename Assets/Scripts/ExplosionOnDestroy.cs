using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class ExplosionOnDestroy : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    ParticleSystem explosionParticles;

    public void Explode(Vector3 velocity)
    {

        ParticleSystem particlesSystem = Instantiate(explosionParticles, transform.position, Quaternion.identity);
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particlesSystem.main.maxParticles];

        particlesSystem.Emit(30);

        int numParticles = particlesSystem.GetParticles(particles);
        Debug.Log("Bullet destroyed - " + velocity);

        for (int i = 0; i < numParticles; i++)
        {
            Vector2 displacement = new Vector2(Random.Range(-Mathf.Abs(velocity.x) * 0.5f + Random.Range(-3f, 3f), Mathf.Abs(velocity.x) * 0.5f), Random.Range(Mathf.Abs(velocity.y) * 0.5f, Mathf.Abs(velocity.y) * 0.9f));

            particles[i].velocity = new Vector2(velocity.x * Random.Range(0f, 1f) + displacement.x, velocity.y * Random.Range(0f, 1f) + displacement.y);
            //particles[i].velocity = new Vector3(0f, 10f, 0f);
        }

        particlesSystem.SetParticles(particles, numParticles);
    }
}
