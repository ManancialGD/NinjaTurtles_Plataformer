using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    // Start is called before the first frame update

    Player playerScript;
    Transform playerTransform;
    Transform thisTransform;
    SpriteRenderer thisSprite;
    void Start()
    {
        playerScript = FindObjectOfType<Player>();
        playerTransform = playerScript.GetComponent<Transform>();
        thisTransform = GetComponent<Transform>();
        thisSprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        thisSprite.flipX = playerTransform.position.x < thisTransform.position.x;
    }
}
