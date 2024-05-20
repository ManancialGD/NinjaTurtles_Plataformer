using UnityEngine;

public class AttackCollisionActivator : MonoBehaviour
{
    [SerializeField] private GameObject slashAttack;
    [SerializeField] private GameObject stabAttack;
    [SerializeField] private GameObject upperCutAttack;
    public int animationAttackType;


    /// <summary>
    /// This code will receive a ID(kinda) from the AnimatorAttacks.cs
    /// And use the information to activate the correct GameObject that has the collision
    /// </summary>
    /// 
    private void Update()
    {
        if (animationAttackType == 1) // Code 1 for SlashAttack
        {
            slashAttack.SetActive(true);

        }
        else if (animationAttackType == 2) // Code 2 for StabAttack
        {
            stabAttack.SetActive(true);
        }
        else if (animationAttackType == 3) // Code 3 for UpperCut
        {
            upperCutAttack.SetActive(true); 
        }
        else // Else, desactivate everything
        {
            slashAttack.SetActive(false);
            stabAttack.SetActive(false);
            upperCutAttack.SetActive(false);
        }
    }
}
