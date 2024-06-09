using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAnimation : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;
    MeleeAttack attack;
    private string currentAnimation;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        attack = GetComponent<MeleeAttack>();
    }

    private void Update()
    {
        if (attack.isAttacking) return;
        
        if (Mathf.Abs(rb.velocity.x) > 0.01f)
        {
            PlayAnimation("MeleeRun");
        }
        else
        {
            PlayAnimation("MeleeIdle");
        }
    }

    public void PlayAnimation(string newAnimation)
    {
        if (newAnimation == currentAnimation) return;
        currentAnimation = newAnimation;
        anim.Play(newAnimation);
    }
}
