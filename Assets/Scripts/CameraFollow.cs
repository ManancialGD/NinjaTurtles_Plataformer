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

    private void Start()
    {
        cameraReaction = MAX_CAMERA_REACTION;
        
        // Encontre o objeto pelo nome do script
        target = GameObject.Find("Player");
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
        else if (updateCameraReaction && cameraReaction == MAX_CAMERA_REACTION) Debug.Log("Camera Reaction is MAX");

        //Debug.Log("Player" + p_changer.CurrentPlayerSelected);
        /*
        if (p_changer.CurrentPlayerSelected < 1 || p_changer.CurrentPlayerSelected > 2)
        {
            Debug.Log("ERRO 854023 - Player index inválido");
            return;
        }

        if (currentPlayerTarget != p_changer.CurrentPlayerSelected)
        {
            target = GameObject.Find("Player" + p_changer.CurrentPlayerSelected);
            currentPlayerTarget = p_changer.CurrentPlayerSelected;
        }
        */

        playerTransform = target.GetComponent<Transform>();
        cameraTranform = GetComponent<Transform>();
        CameraPosition = new Vector2(cameraTranform.position.x, cameraTranform.position.y);

        float distanceX = playerTransform.position.x - CameraPosition.x;
        float distanceY = playerTransform.position.y - CameraPosition.y;

        float distance = Mathf.Abs(distanceX + distanceY);
        //Debug.Log("distance = " + distance);

        rb.velocity = new Vector2(rb.velocity.x * cameraResistance + distanceX * cameraReaction, rb.velocity.y * cameraResistance + distanceY * cameraReaction);

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

}