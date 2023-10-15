using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Resource", menuName = "Scriptable Objects/Resource", order = 4)]
public class ResourceSourceData : ScriptableObject
{
    public string code;
    public string resourceName;
    public int quantity;
    public GameObject prefab;
    public int tier;
    public float baseGatheringTime; //in seconds for 1 resource
}

