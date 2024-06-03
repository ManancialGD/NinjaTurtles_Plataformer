using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceAnimationUI : MonoBehaviour
{
    Animator anim;
    [SerializeField] private bool isBlinking = false;
    [SerializeField] private int timeIndex = 0;
    [SerializeField] private int randomNumber;
    [SerializeField] private int randomBlink;
    [SerializeField] private bool blinkedTwice = true;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        randomNumber = Random.Range(350, 500);
    }

private void FixedUpdate()
    {
        timeIndex ++;

        if (((timeIndex == randomNumber) || timeIndex > randomNumber) && !isBlinking)
        {
            isBlinking = true;
            if (!blinkedTwice) randomBlink = Random.Range(1, 10);
            else randomBlink = 10;
            
            if ( randomBlink == 1)
            {
                blinkedTwice = true;
                randomNumber = 10;
            }
            else
            {
                randomNumber = Random.Range(350, 500);
                blinkedTwice = false;
            }
            anim.SetBool("Blink", true);

            timeIndex = 0;
        }
        else if (isBlinking && (timeIndex >= 10))
        {
            isBlinking = false;
            anim.SetBool("Blink", false);

            timeIndex = 0;
        }

    }
}