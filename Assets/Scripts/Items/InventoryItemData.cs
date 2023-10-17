using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a scriptable object that defines what an item is in the game
/// It can be inherited from to have branched version of items
/// </summary>


[CreateAssetMenu(menuName = "Scriptable Objects/Inventory Item")]

public class InventoryItemData : ScriptableObject
{
    public int ID;
    public string DisplayName;
    // public Sprite Icon;
    public int MaximumStackSize;
    [TextArea(4,4)]
    public string Description;

}
