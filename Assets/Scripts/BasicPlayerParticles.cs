using UnityEngine;

public class BasicPlayerParticles : MonoBehaviour
{

    static Vector2 playerSize = new Vector2(0.345f, 0.95f);
    private ParticleSystem playerParticle;
    Player playerScript;
    ParticleSystem.MainModule mainModule;

    void Start()
    {
        
        playerParticle = GetComponent<ParticleSystem>();
        playerScript = FindAnyObjectByType<Player>(); //Iremos alterar no futuro

        mainModule = playerParticle.main;
        mainModule.startColor = Color.black;

    }

    public void CreateParticle(int particlesQuantity, string spawnType, Vector2 velocity, float[] rangeX, float[] rangeY, float[] sideCorrection, Color particleColor)
    {
        mainModule.startColor = particleColor;

        Vector2 initialPosition;
        if (spawnType.ToLower() == "down") initialPosition = new Vector2(playerScript.rb.position.x, playerScript.rb.position.y - playerSize.y / 2);
        else if (spawnType.ToLower() == "right") initialPosition = new Vector2(playerScript.rb.position.x + playerSize.x / 2, playerScript.rb.position.y - playerSize.y / 2);
        else if (spawnType.ToLower() == "left") initialPosition = new Vector2(playerScript.rb.position.x - playerSize.x / 2, playerScript.rb.position.y - playerSize.y / 2);
        else
        {
            Debug.Log("ERROR 6128241 - Invalid input");
            return;
        }
       
        Vector3 particlePosition = initialPosition;
        
        playerParticle.transform.position = particlePosition;
        playerParticle.Emit(particlesQuantity);
       
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[playerParticle.main.maxParticles];
        int numParticles = playerParticle.GetParticles(particles);

        for (int i = 0; i < numParticles; i++)
        {
            float newSideCorrection = Random.Range(sideCorrection[0], sideCorrection[1]);
            particles[i].velocity = new Vector3(velocity.x * Random.Range(rangeX[0], rangeX[1]) + newSideCorrection, velocity.y * Random.Range(rangeY[0], rangeY[1]), 0f);
        }

        playerParticle.SetParticles(particles, numParticles);
    }

}


