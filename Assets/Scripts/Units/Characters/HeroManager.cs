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

    public GameObject battleIcon;

    public bool canBeMovedByPlayer;
    public EnemyUnitManager CurrentTarget;

    protected override void Awake()
    {
        base.Awake();
        battleIcon.SetActive(false);
        Attack = unitData.damages;
        AttackSpeed = unitData.attackSpeed;
        AttackRange = unitData.attackRange;
    }

    public void InflictDamage(EnemyUnitManager target, int damage)
    {
        if(target != null)
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

}
