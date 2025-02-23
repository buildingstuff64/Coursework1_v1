using Assets.Scripts.Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageComponent : MonoBehaviour, Idamageable
{
    public BaseEnemy enemy;
    public float damageReductionPercent = 1;
    public float takeDamage(float damage)
    {
        enemy.takeDamage(damage * damageReductionPercent);
        return damage * damageReductionPercent;
    }

    public void setEnemy(BaseEnemy e)
    {
        enemy = e;
    }

}
