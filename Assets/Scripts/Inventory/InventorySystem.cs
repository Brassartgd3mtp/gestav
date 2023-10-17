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

    public InventorySystem(int _size) //Constructor that sets the amount of stacks
    {
        inventorySlots = new List<InventorySlot>(_size);

        for (int i = 0; i < _size; i++)
        {
            inventorySlots.Add(new InventorySlot());
        }
    }

    public bool AddToInventory(InventoryItemData _itemToAdd, int _amountToAdd)
    {
        if(HasFreeSlot(out InventorySlot _freeSlot)) //Gets the first avaliable slot
        {
            _freeSlot.UpdateInventorySlot(_itemToAdd, _amountToAdd);
            OnInventorySlotChanged?.Invoke(_freeSlot);
            return true;
        }

        return false;
    }

    public bool HasFreeSlot(out InventorySlot _freeSlot)
    {
    _freeSlot = InventorySlots.FirstOrDefault(i => i.ItemData == null);
    return _freeSlot == null ? false : true;
    }

    public bool ContainItem(InventoryItemData _itemToAdd, out List<InventorySlot> _invSlot)
    {
        _invSlot = inventorySlots.Where(i => i.ItemData == _itemToAdd).ToList();

        return _invSlot.Count > 1 ? true : false;
    }
}
