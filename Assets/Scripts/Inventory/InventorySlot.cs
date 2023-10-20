using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    [SerializeField] private InventoryItemData itemData; //reference to the data of what an item is
    [SerializeField] private int stackSize; //current stacksize of the slot

    public InventoryItemData ItemData => itemData;
    public int StackSize => stackSize;

    public InventorySlot(InventoryItemData _source, int _amount) //constructor to make a occupied inventory slot
    {
        itemData = _source;
        stackSize = _amount;
    }

    public InventorySlot() //constructor to make a blank inventory slot
    {
        ClearSlot();
    }

    public void ClearSlot()
    {
        itemData = null;
        stackSize = -1;
    }

    public bool RoomLeftInStack(int _amountToAdd, out int _amountRemaining)
    {
        _amountRemaining = ItemData.MaximumStackSize - stackSize;
        return RoomLeftInStack(_amountToAdd);
    }

    public bool RoomLeftInStack(int _amountToAdd)
    {
        if (itemData == null || itemData != null && stackSize + _amountToAdd <= itemData.MaximumStackSize) return true;
        else return false;
    }

    public void AddToStack(int _amount)
    {
        stackSize += _amount;
    }

    public void RemoveFromStack(int _amount)
    {
        stackSize -= _amount;
    }

    public void UpdateInventorySlot(InventoryItemData _data, int _amount)
    {
        itemData = _data;
        stackSize -= _amount;
    }
}
