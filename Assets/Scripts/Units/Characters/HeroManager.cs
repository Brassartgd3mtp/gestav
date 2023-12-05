using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HeroManager : CharacterManager
{
    [Header("Statistics")]
    public int Attack;
    public float AttackSpeed;


    private bool inIsBattle;
    public bool IsInBattle => inIsBattle;

    public bool canBeMovedByPlayer;
    public EnemyManager CurrentTarget;

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
    private void Update()
    {
        Debug.Log(CurrentTarget);
    }
}
