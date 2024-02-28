using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private string nomeDoScriptPlayer = "Player";

    private Transform alvo;
    private float newX;
    private float newY;
    private Transform CameraTransform;

    [SerializeField] public float CameraHeightDisplay;
    [SerializeField] public float CameraLimitY;

    void Start()
    {
        // Encontrar o objeto com o script "Player" pelo nome
        GameObject jogador = GameObject.Find(nomeDoScriptPlayer);
        CameraTransform = GetComponent<Transform>();

        // Verificar se o objeto foi encontrado
        if (jogador != null)
        {
            // Obter o componente Transform do objeto "Player"
            alvo = jogador.GetComponent<Transform>();
            newX = alvo.position.x;
            newY = alvo.position.y;
            transform.position = new Vector3(newX, newY, CameraTransform.position.z);
        }
        else
        {
            Debug.LogError("Objeto com o script Player não encontrado na cena.");
        }
    }

    void Update()
    {
        // Verificar se o alvo está definido
        if (alvo != null)
        {
            newX += (alvo.position.x - CameraTransform.position.x) / 80;

            if(Mathf.Abs(alvo.position.y + CameraHeightDisplay - CameraTransform.position.y) > CameraLimitY){
                //Debug.Log(Mathf.Abs(alvo.position.y + CameraHeightDisplay - CameraTransform.position.y));
                newY += (alvo.position.y + CameraHeightDisplay - CameraTransform.position.y) / 80;
            } else if(alvo.position.y + CameraHeightDisplay < CameraTransform.position.y){
                newY += (alvo.position.y + CameraHeightDisplay - CameraTransform.position.y) / 5;
            }
            


            // Atualizar a posição da câmera para seguir o objeto "Player"
            transform.position = new Vector3(newX, newY, transform.position.z);
        }
    }
}