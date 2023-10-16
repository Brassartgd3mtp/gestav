using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
