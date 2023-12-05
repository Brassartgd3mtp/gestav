using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroManager : CharacterManager
{
    [Header("Statistics")]
    public int Attack;
    public float AttackSpeed;
    protected override void Awake()
    {
        base.Awake();
        Attack = unitData.damages;
        AttackSpeed = unitData.attackSpeed;
    }

    private void InflictDamage(EnemyManager target, int damage)
    {
        damage = Attack;
        target.HealthPoints -= damage;
        target.HealthUpdate();
    }
}
