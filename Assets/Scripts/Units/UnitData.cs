using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "Scriptable Objects/Unit", order = 1)]
public class UnitData : ScriptableObject
{
    public string code;
    public string unitName;
    public int healthPoints;
    public GameObject prefab;
    public ItemTypeAndCount[] resourcesToBuild;
    public int damages;
    public float attackSpeed;
    public float moveSpeed;
    public float attackRange;
    public float gatheringTime;
}
