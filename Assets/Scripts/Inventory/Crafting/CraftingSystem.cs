using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class CraftingSystem : MonoBehaviour
{
    [SerializeField] private CraftingInventory inventory;
    [SerializeField] private InventoryResourceType resourceToCraft;
    [SerializeField] private InventoryResourceType resourceNeeded;

    private int resourcesInReserve =0;
    public int GetAmountOfItemsInReserve()
    {
        foreach (InventorySlot slot in inventory.InventorySystem.InventorySlots)
        {
            if(slot.ItemData.resourceType == resourceToCraft)
            {
                resourcesInReserve++;
            }
        }
        return resourcesInReserve;
    }
}
