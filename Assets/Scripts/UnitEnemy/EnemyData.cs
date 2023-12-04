using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Scriptable Objects/Enemy", order = 1)]
public class EnemyData : ScriptableObject
{
    public string code;
    public string unitName;
    public int healthPoints;
    public GameObject prefab;

}