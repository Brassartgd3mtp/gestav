using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "Scriptable Objects/Building", order = 1)]
public class BuildingData : ScriptableObject
{
    public string code;
    public string unitName;
    public int healthpoints;
    public GameObject prefab;

}
