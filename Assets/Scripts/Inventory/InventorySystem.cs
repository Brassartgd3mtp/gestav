using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

[System.Serializable]

public class InventorySystem
{
    [SerializeField] private List<InventorySlot> inventorySlots;

    public List<InventorySlot> InventorySlots => inventorySlots;
    public int InventorySize => InventorySlots.Count;

    public UnityAction<InventorySlot> OnInventorySlotChanged;

    public InventorySystem(int _size)
    {
        inventorySlots = new List<InventorySlot>(_size);

        for (int i = 0; i < _size; i++)
        {
            inventorySlots.Add(new InventorySlot());
        }
    }

    public bool AddToInventory(InventoryItemData _itemToAdd, int _amountToAdd)
    {
        inventorySlots[0] = new InventorySlot(_itemToAdd, _amountToAdd);
            return true;
    }

}
