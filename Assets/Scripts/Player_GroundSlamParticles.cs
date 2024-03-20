using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_GroundSlamParticles : MonoBehaviour
{
    // Start is called before the first frame update
    ParticleSystem particles;
    void Start()
    {
        particles = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        Debug.Log(".");
    }

    public void DisplayGroundSlamParticles(Vector2 position)
    {
        if (particles == null)
        {
            Debug.Log("ERROR 954921 - Player_GroundSlamParticles ParticleSystem not found");
            return;
        }
        transform.position = new Vector2(position.x, position.y);
        particles.Play();
        Debug.Log("Ground Slam Particles");

    }
}
