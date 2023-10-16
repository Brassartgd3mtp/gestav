using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    [SerializeField] private InventoryItemData itemData;
    [SerializeField] private int stackSize;

    public InventoryItemData ItemData => itemData;
    public int StackSize => stackSize;

    public InventorySlot(InventoryItemData _source, int _amount)
    {
        itemData = _source;
        stackSize = _amount;
    }

    public InventorySlot()
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
        if (stackSize + _amountToAdd <= itemData.MaximumStackSize) return true;
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
}
