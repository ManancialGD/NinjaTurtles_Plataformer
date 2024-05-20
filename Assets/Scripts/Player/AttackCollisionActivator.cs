using UnityEngine;

public class AttackCollisionActivator : MonoBehaviour
{
    [SerializeField] private GameObject slashAttack;
    [SerializeField] private GameObject stabAttack;
    [SerializeField] private GameObject upperCutAttack;
    public int animationAttackType;
    private void Update()
    {
        //Debug.Log(animationAttackType);
        if (animationAttackType == 1)
        {
            slashAttack.SetActive(true);

        }
        else if (animationAttackType == 2)
        {
            stabAttack.SetActive(true);
        }
        else if (animationAttackType == 3)
        {
            upperCutAttack.SetActive(false);
        }
        else
        {
            slashAttack.SetActive(false);
            stabAttack.SetActive(false);
            upperCutAttack.SetActive(false);
        }
    }
}
