using UnityEngine;

public class BasicPlayerParticles : MonoBehaviour
{

    static Vector2 playerSize = new Vector2(8, 28);
    private ParticleSystem playerParticle;
    Player playerScript;
    ParticleSystem.MainModule mainModule;
    NativeInfo native;

    void Start()
    {
        native = FindObjectOfType<NativeInfo>();
        playerParticle = GetComponent<ParticleSystem>();
        playerScript = native.GetPlayerObj(native.currentPlayerID).GetComponent<Player>();

        mainModule = playerParticle.main;
        mainModule.startColor = Color.black;

    }

    void Update()
    {
        playerScript = native.GetPlayerObj(native.currentPlayerID).GetComponent<Player>();
    }

    public void CreateParticle(int particlesQuantity, string spawnType, Vector2 velocity, float[] rangeX, float[] rangeY, float[] sideCorrection, Color particleColor)
    {
        mainModule.startColor = particleColor;
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[playerParticle.main.maxParticles];
        int numInitialParticles = playerParticle.GetParticles(particles);

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


        int numParticles = playerParticle.GetParticles(particles);

        for (int i = numInitialParticles; i < numParticles; i++)
        {
            float newSideCorrection = Random.Range(sideCorrection[0], sideCorrection[1]);
            particles[i].velocity = new Vector3(velocity.x * Random.Range(rangeX[0], rangeX[1]) + newSideCorrection, velocity.y * Random.Range(rangeY[0], rangeY[1]), 0f);
        }

        playerParticle.SetParticles(particles, numParticles);
    }

}


