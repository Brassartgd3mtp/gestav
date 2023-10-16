using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Inventory", menuName ="Inventory System/Inventory")]

public class InventoryObject : ScriptableObject
{
    public static List<InventorySlot> Container = new List<InventorySlot>();

    public void AddItem(ItemObject _item, int _amount)
    {
        Container.Add(new InventorySlot(_item, _amount));
    }
}

[System.Serializable]
public class InventorySlot
{
    public ItemObject Item;
    public int Amount;

    public InventorySlot(ItemObject _item, int _amount)
    {
        Item = _item;
        Amount = _amount;
    }

    public void AddIntValue(int value)
    {
        Amount += value;
    }
}