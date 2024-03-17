using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParticles : MonoBehaviour
{

    static Vector2 playerSize = new Vector2(0.345f, 0.95f);
    Transform player;
    Rigidbody2D rb;
    Player playerScript;
    Vector2 position_history;
    Vector2 velocity_history;
    Collision coll;
    public Sprite BallSprite;
    int LastParticleID = 0;
    public bool enableWalkParticles;

    //float time = 0f;

    public float opacityChangeTime;

    Dictionary<int, (float, SpriteRenderer)> particles = new Dictionary<int, (float, SpriteRenderer)>();
    GameObject[] particles_obj = new GameObject[0];
    // ID - ( lifeTime, SpriteRenderer ) 

    void Start()
    {
        playerScript = FindObjectOfType<Player>();
        coll = playerScript.GetComponent<Collision>();
        rb = playerScript.GetComponent<Rigidbody2D>();
        player = playerScript.GetComponent<Transform>();
        position_history = new Vector2(player.position.x, player.position.y);
        velocity_history = new Vector2(rb.velocity.x, rb.velocity.y);
    }


    void Update()
    {

        // Calcular as particulas existentes
        Dictionary<int, (float, SpriteRenderer)> particlesCopy = new Dictionary<int, (float, SpriteRenderer)>(particles);
        foreach (KeyValuePair<int, (float, SpriteRenderer)> par in particlesCopy)
        {

            int p_id = par.Key;
            float p_lifeTime = par.Value.Item1 - Time.deltaTime;
            if (p_lifeTime <= 0f)
            {
                particles.Remove(p_id);
                Destroy(particles_obj[p_id]);
                continue;
            }

            SpriteRenderer p_spriteRenderer = par.Value.Item2;

            particles[p_id] = (p_lifeTime, particles[p_id].Item2);

            if (p_lifeTime < opacityChangeTime) ChangeSpriteOpacity(p_spriteRenderer, p_lifeTime * (1 / opacityChangeTime));

        }


        if (!enableWalkParticles) return;
        int particlesNumber = 0;

        Vector2 velocityDifference = new Vector2(velocity_history.x - rb.velocity.x, velocity_history.y - rb.velocity.y);

        if (!coll.onGround || Mathf.Abs(velocityDifference.x) > 2f || Mathf.Abs(rb.velocity.y) > 0.2f) return;

        particlesNumber = (int)Mathf.Abs(velocityDifference.x * 5.5f);

        for (int i = 0; i < particlesNumber; i++)
        {
            Vector2 randomVector = new Vector2(Random.Range(velocityDifference.x * -0.1f, velocityDifference.x * 0.3f), Random.Range(2f, 3f));
            float randomMultiplier = Random.Range(velocityDifference.x * 1.5f, velocityDifference.x * 3f);

            CreateParticle(opacityChangeTime, "down", new Vector2(velocityDifference.x * randomMultiplier + randomVector.x, randomVector.y));

        }


        // Atualizar as variáveis
        position_history = new Vector2(player.position.x, player.position.y);
        velocity_history = new Vector2(rb.velocity.x, rb.velocity.y);
    }

    public void ChangePlayerTarget(Transform new_playerScript)
    { // Não foi testado ainda
        player = new_playerScript;
        position_history = new Vector2(0f, 0f);
        velocity_history = new Vector2(0f, 0f);
    }

    public void CreateParticle(float lifeTime, string spawnType, Vector2 initialVelocity)
    {
        Vector2 initialPosition;
        if (spawnType.ToLower() == "down") initialPosition = new Vector2(rb.position.x, rb.position.y - playerSize.y / 2);
        else if (spawnType.ToLower() == "right") initialPosition = new Vector2(rb.position.x + playerSize.x / 2, rb.position.y - playerSize.y / 2);
        else if (spawnType.ToLower() == "left") initialPosition = new Vector2(rb.position.x - playerSize.x / 2, rb.position.y - playerSize.y / 2);
        else
        {
            Debug.Log("ERRO 6128241 - Invalid input");
            return;
        }

        GameObject newSpriteObject = new GameObject("Particle_" + LastParticleID);
        newSpriteObject.transform.SetParent(GetComponent<Transform>());
        Rigidbody2D p_rb = newSpriteObject.AddComponent<Rigidbody2D>();
        p_rb.gravityScale = 2.5f;
        SpriteRenderer spriteRenderer = newSpriteObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = BallSprite;
        newSpriteObject.transform.position = initialPosition;
        p_rb.velocity = new Vector2(initialVelocity.x, initialVelocity.y);
        newSpriteObject.transform.localScale = new Vector2(0.2f, 0.2f);

        particles.Add(LastParticleID, (lifeTime, spriteRenderer));

        // Atualizar particles_obj
        GameObject[] debug_particles = new GameObject[particles_obj.Length + 1];
        for (int i = 0; i < particles_obj.Length; i++)
        {
            debug_particles[i] = particles_obj[i];
        }
        debug_particles[particles_obj.Length] = newSpriteObject;

        particles_obj = debug_particles;

        LastParticleID++;
        Debug.Log("Particula criada - " + lifeTime + " | (" + initialVelocity.x + ", " + initialVelocity.y + ")");
        return;
    }

    void ChangeSpriteOpacity(SpriteRenderer renderer, float opacity)
    {
        Color color = renderer.color;
        color.a = opacity; // Defina a opacidade desejada
        renderer.color = color; // Atribua a nova cor ao componente SpriteRenderer
    }

}
