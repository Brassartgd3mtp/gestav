using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSource
{
    protected ResourceSourceData data;
    protected Transform resourceTransform;
    protected int currentAmount;

    public ResourceSource(ResourceSourceData _data)
    {
        data = _data;
        currentAmount = _data.quantity;

        // Instantiate a GameObject based on the unit's code from a prefab
        GameObject g = GameObject.Instantiate(data.prefab) as GameObject;
        resourceTransform = g.transform;
    }

    public ResourceSourceData Data { get => data; }

    public string Code { get => data.code; }
    public Transform Transform { get => resourceTransform; }
    public int ResourcesRemaining { get => currentAmount; set => currentAmount = value; }
    public int MaxResources { get => data.quantity; }
    public int Tier { get => data.tier; }
    public float BaseGatheringTime { get => data.baseGatheringTime; }
}
