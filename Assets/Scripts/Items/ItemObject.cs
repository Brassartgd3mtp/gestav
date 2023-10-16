using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    GameResource,
    Equipment,
    Default
}


public abstract class ItemObject : ScriptableObject
{
    public string Name;
    public GameObject prefab;
    public ItemType type;

    [TextArea(15,20)]
    public string Description;
}
