using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeoHP : MonoBehaviour
{
    private int hp;
    public int HP { get { return hp; } private set { hp = value; if (hp <= 0) Die(); else if (hp > maxHP) hp = maxHP; } }
    private int maxHP = 100;
    

    public void Heal(int healAmount)
    {
        HP += healAmount;
    }
    public void Damage(int damageAmount)
    {
        HP -= damageAmount;
    }
    private void Die()
    {
        Destroy(gameObject);
    }
}
