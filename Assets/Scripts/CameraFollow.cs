using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    GameObject target;
    Vector3 velocity = Vector3.zero;

    //public PlayerChanger p_changer;

    [Range(0, 1)]
    public float smoothTime;
    public Vector3 positionOffset;

    [Header("Axis Limitation")]
    public Vector2 xLimit;
    public Vector2 yLimit;

    public Vector2 CameraPosition;

    //int currentPlayerTarget;

    Rigidbody2D rb;
    Transform cameraTranform;
    Transform playerTransform;

    public float cameraResistance = 0.98f;
    public float MAX_CAMERA_REACTION;
    float cameraReaction = 0f; //MAX 1;
    bool updateCameraReaction = true;
    float reactionTimer;

    public bool onCombatMode = false;
    Vector2 enemyPosition;

    bool cameraShaking;
    float cameraShakeTransitionTime;
    Vector2 cameraShakeVelocity;
    float cameraShakeCooldown;
    float cameraShakeRotation;
    float cameraShakeResistence;
    NativeInfo native;

    private void Start()
    {
        cameraShaking = false;
        cameraReaction = MAX_CAMERA_REACTION;

        native = FindObjectOfType<NativeInfo>();

        // Encontre o objeto pelo nome do script
        target = native.playerObj[native.currentPlayerID - 1];
        playerTransform = target.GetComponent<Transform>();
        //currentPlayerTarget = 1;

        //Logs
        if (target != null)
        {
            Debug.Log("Player encontrado com sucesso!");
        }
        else
        {
            Debug.LogError("Erro: Player não encontrado na cena. (target == null)");
            return;
        }
        rb = GetComponent<Rigidbody2D>();
        CameraPosition = new Vector2(playerTransform.position.x, playerTransform.position.y);
        cameraTranform = GetComponent<Transform>();
    }

    void Update()
    {
        if (target != native.playerObj[native.currentPlayerID - 1]) // changed player
        {
            target = native.playerObj[native.currentPlayerID - 1];
        }
    }

    private void LateUpdate()
    {

        if (reactionTimer > 0f) reactionTimer -= Time.deltaTime;
        if (reactionTimer < 0f)
        {
            reactionTimer = 0f;
            if (!updateCameraReaction) updateCameraReaction = true;
        }

        // Atualizar a "Camera Reaction" para voltar ao normal
        if (updateCameraReaction && cameraReaction != MAX_CAMERA_REACTION)
        {
            if (cameraReaction == 0f) cameraReaction = 0.01f;
            else if (cameraReaction < MAX_CAMERA_REACTION)
            {
                cameraReaction += MAX_CAMERA_REACTION * 0.4f * Time.deltaTime; //99 frames para voltar ao normal.
                if (cameraReaction > MAX_CAMERA_REACTION) cameraReaction = MAX_CAMERA_REACTION;
            }
            else
            {
                cameraReaction -= MAX_CAMERA_REACTION * 0.4f * Time.deltaTime; //99 frames para voltar ao normal.
                if (cameraReaction < MAX_CAMERA_REACTION) cameraReaction = MAX_CAMERA_REACTION;
            }

        }
        //else if (updateCameraReaction && cameraReaction == MAX_CAMERA_REACTION) Debug.Log("Camera Reaction is MAX");

        //Debug.Log("Player" + p_changer.CurrentPlayerSelected);
        /*
        if (p_changer.CurrentPlayerSelected < 1 || p_changer.CurrentPlayerSelected > 2)
        {
            Debug.Log("ERROR 854023 - Player index inválido");
            return;
        }

        if (currentPlayerTarget != p_changer.CurrentPlayerSelected)
        {
            target = GameObject.Find("Player" + p_changer.CurrentPlayerSelected);
            currentPlayerTarget = p_changer.CurrentPlayerSelected;
        }
        */

        if (target == null)
        {
            Debug.Log("ERROR 862841 - Invalid target GameObject");
            return;
        }
        playerTransform = target.GetComponent<Transform>();
        cameraTranform = GetComponent<Transform>();
        CameraPosition = new Vector2(cameraTranform.position.x, cameraTranform.position.y);

        float distanceX;
        float distanceY;

        if (onCombatMode)
        {

            Vector2 combatDistance = new Vector2(CameraPosition.x - enemyPosition.x, CameraPosition.y - enemyPosition.y);

            distanceX = playerTransform.position.x - (CameraPosition.x + combatDistance.x / 2);
            distanceY = playerTransform.position.y - (CameraPosition.y + combatDistance.y / 2);
        }
        else
        {
            distanceX = playerTransform.position.x - CameraPosition.x;
            distanceY = playerTransform.position.y - CameraPosition.y;
        }

        float distance = Mathf.Abs(distanceX + distanceY);
        //Debug.Log("distance = " + distance);

        if (!cameraShaking) rb.velocity = new Vector2(rb.velocity.x * cameraResistance + distanceX * cameraReaction, rb.velocity.y * cameraResistance + distanceY * cameraReaction);
        else rb.velocity = new Vector2(rb.velocity.x * cameraShakeResistence + distanceX * cameraReaction, rb.velocity.y * cameraResistance + distanceY * cameraReaction);

        onCombatMode = false;
    }

    public void BoostCamera(Vector2 force)
    {
        rb.velocity = new Vector2(rb.velocity.x + force.x, rb.velocity.y + force.y);
        return;
    }

    public void SetCameraReaction(float new_reaction, float update_delay)
    {
        cameraReaction = new_reaction;
        reactionTimer = update_delay;
        return;
    }

    public void CombatMode(float x, float y)
    {
        enemyPosition = new Vector2(x, y);
        onCombatMode = true;
    }

    public void ChangeCameraTarget(GameObject newTarget)
    {
        target = newTarget;
    }

    public void CameraShake(float shakeTime, float shakeInterval, float shakeResistence, Vector2 velocityIntensity)
    {
        if (!cameraShaking) cameraShaking = true;
        cameraShakeCooldown = shakeTime;
        cameraShakeResistence = shakeResistence;

        StartCoroutine(SetCameraShakeProperties(0f, shakeInterval, velocityIntensity));
        StartCoroutine(FinishCameraShake(shakeTime));
        return;
    }

    /*
        private IEnumerator InitiateCameraShakeDefault(float cooldown)
        {

            yield return new WaitForSeconds(cooldown);
            CameraShake(0.2f, 0.05f, 0.99f, new Vector2(10f, 0f));
        }
    */

    public void DamageCameraShake()
    {
        CameraShake(0.15f, 0.05f, 0.99f, new Vector2(8f, 3f));
    }

    private IEnumerator SetCameraShakeProperties(float waitTime, float shakeInterval, Vector2 velocityIntensity)
    {
        yield return new WaitForSeconds(waitTime);

        Debug.Log("Camera Shake started");

        int num = 1;

        Vector2 currentDisplacement = new Vector2(0f, 0f);

        while (cameraShaking)
        {
            if (cameraShakeCooldown <= 0f)
            {
                cameraShaking = false;
                cameraShakeCooldown = 0f;
                break;
            }

            currentDisplacement += num * velocityIntensity;
            rb.velocity += num * velocityIntensity;
            num = -num;
            yield return new WaitForSeconds(shakeInterval);
        }

        rb.velocity -= currentDisplacement;

        Debug.Log("Camera Shake finished");

    }

    private IEnumerator FinishCameraShake(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        cameraShakeCooldown = 0f;
        cameraShaking = false;

    }

    //CameraShake()

}