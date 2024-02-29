using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    GameObject target;
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
        target = GameObject.Find("Player");

        if (target != null)
        {
            Debug.Log("Player encontrado com sucesso!");
        }
        else
        {
            Debug.LogError("Erro: Player não encontrado na cena.");
        }
    }

    private void LateUpdate()
    {
        Vector3 targetPosition = target.transform.position + positionOffset;
        targetPosition = new Vector3(Mathf.Clamp(targetPosition.x, xLimit.x, xLimit.y), Mathf.Clamp(targetPosition.y, yLimit.x, yLimit.y), positionOffset.z);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
}
