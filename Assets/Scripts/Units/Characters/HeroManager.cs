using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HeroManager : CharacterManager
{
    [Header("Statistics")]
    public int Attack;
    public float AttackSpeed;
    public float AttackRange;

    public GameObject battleIcon;
    public Slider attackCdSlider;

    public bool canBeMovedByPlayer;
    public EnemyUnitManager CurrentTarget;

    protected override void Awake()
    {
        base.Awake();
        battleIcon.SetActive(false);
        Attack = unitData.damages;
        AttackSpeed = unitData.attackSpeed;
        AttackRange = unitData.attackRange;

        attackCdSlider.maxValue = 1 / unitData.attackSpeed;
        attackCdSlider.value = 0f;
        attackCdSlider.gameObject.SetActive(false);
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
