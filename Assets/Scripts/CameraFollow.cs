using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Player playerScript; // Reference to the Player script
    Vector3 velocity = Vector3.zero;

    [Range(0, 1)]
    public float smoothTime;
    public Vector3 positionOffset;
    [Header("Axis Limitation")]
    public Vector2 xLimit;
    public Vector2 yLimit;

    private void Awake()
    {
        // Encontre o objeto pelo nome do script
        playerScript = FindObjectOfType<Player>();

        //Logs
        if (playerScript != null)
        {
            Debug.Log("Player encontrado com sucesso!");
        }
        else
        {
            Debug.LogError("Erro: Script do Player não encontrado na cena.");
        }
    }

    private void LateUpdate()
    {
        if (playerScript != null)
        {
            Vector3 targetPosition = playerScript.transform.position + positionOffset;
            targetPosition = new Vector3(Mathf.Clamp(targetPosition.x, xLimit.x, xLimit.y), Mathf.Clamp(targetPosition.y, yLimit.x, yLimit.y), positionOffset.z);
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
    }
}
