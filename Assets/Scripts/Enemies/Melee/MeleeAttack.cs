using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    MeleeAnimation anim;
    public bool isAttacking;
    [SerializeField] private float attackArea = 80f;
    public bool IsLeoInArea { get; private set; }


    private void Awake()
    {
        anim = GetComponent<MeleeAnimation>();
    }
    private void Update()
    {
        // Check if Leo is within the collision radius
        Collider2D hitCollider = Physics2D.OverlapCircle(transform.position, attackArea, LayerMask.GetMask("Leo"));
        IsLeoInArea = hitCollider != null;

        if (IsLeoInArea)
        {
            anim.PlayAnimation("MeleeAttack");
        }
    }
}
