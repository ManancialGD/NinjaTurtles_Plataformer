using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    MeleeAnimation anim;
    EnemyHP hp;
    public bool isAttacking;
    [SerializeField] private float attackArea = 80f;
    public bool IsLeoInArea { get; private set; }
    bool canAttack;

    private void Awake()
    {
        anim = GetComponent<MeleeAnimation>();
        hp = GetComponent<EnemyHP>();
        canAttack = true;
    }
    private void Update()
    {
        // Check if Leo is within the collision radius
        Collider2D hitCollider = Physics2D.OverlapCircle(transform.position, attackArea, LayerMask.GetMask("Leo"));
        if (hitCollider != null) StartCoroutine(WaitALittleToAttack());
        else IsLeoInArea = false;

        if (IsLeoInArea)
        {
            if (canAttack && !hp.IsStunned)
            {
                anim.PlayAnimation("MeleeAttack");
                StartCoroutine(AttackCooldown());
            }
        }
    }

    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(1);
        canAttack = true;
    }
    IEnumerator WaitALittleToAttack()
    {
        yield return new WaitForSeconds(.5f);
        IsLeoInArea = true;
    }
}
