using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BuildingInventory : UnitInventory, IInteractable
{
    public List<ItemTypeAndCount> items = new List<ItemTypeAndCount>();
    private int ResourcesUsable;
    protected override void Awake()
    {
        base.Awake();
        ResourcesUsable = 0;
    }
   private void Update()
    {
    //    KnowResourcesInInv();
    } 


    public int KnowResourcesInInv()
    {
        ResourcesUsable = 0;
        for (int i = 0; i<inventorySystem.InventorySlots.Count; i++)
        {
            if (inventorySystem.InventorySlots[i].ItemData.resourceType == validType)
            {
                ResourcesUsable++;
                Debug.Log(ResourcesUsable);
            }
        }
        return ResourcesUsable;
    }

    //Method that gets all the items in the inventory and there count
    public List<ItemTypeAndCount> GetAllItems()
    {
        items.Clear();

        foreach(InventorySlot slot in inventorySystem.InventorySlots) 
        {
            int i = 0;
            bool wasItemAdded = false;
            foreach(ItemTypeAndCount itemAndCount in items)
            {
                if(itemAndCount.item == slot.ItemData)
                {
                    items[i].count += 1;
                    wasItemAdded = true;
                }

                i++;
            }

        if( !wasItemAdded )
            {
                items.Add(new ItemTypeAndCount(slot.ItemData, 1));
            }
        }
        Debug.Log(items);
        return items;
    }


}
