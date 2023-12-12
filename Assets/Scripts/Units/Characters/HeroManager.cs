using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HeroManager : CharacterManager
{
    [Header("Statistics")]
    public int Attack;
    public float AttackSpeed;
    public float AttackRange;


    private bool inIsBattle;
    public bool IsInBattle => inIsBattle;

    public bool canBeMovedByPlayer;
    public EnemyUnitManager CurrentTarget;

    protected override void Awake()
    {
        base.Awake();
        Attack = unitData.damages;
        AttackSpeed = unitData.attackSpeed;
        AttackRange = unitData.attackRange;
    }

    public void InflictDamage(EnemyUnitManager target, int damage)
    {
        damage = Attack;
        target.HealthPoints -= damage;
        target.HealthUpdate();
        if (target.HealthPoints <= 0) 
        {
        CurrentTarget = null;
        }
    }

}
