using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnitInventory : InventoryHolder, IInteractable
{

    protected override void Awake()
    {
        base.Awake();
        Global.allInventories.Add(this);
    }

    protected virtual void OnDestroy()
    {
        Global.allInventories.Remove(this);
    }

    public UnityAction<IInteractable> OnInteractionComplete {  get; set; }


    public void Interact(GameManager interactor, out bool interactionWasSuccessful)
    {
        OnDynamicInventoryDisplayRequested?.Invoke(InventorySystem);
        interactionWasSuccessful = true;
    }
    public void EndInteraction()
    {

    }


    public void TransferItems(UnitInventory inventoryGiver, UnitInventory inventoryReceiver)
    {
        for(int i = 0;i<inventoryGiver.InventorySystem.InventorySize;i++)
        {
            if( inventoryReceiver.validType.Contains(inventoryGiver.InventorySystem.InventorySlots[i].ItemData.resourceType))
            {
                inventoryReceiver.InventorySystem.AddToInventory(inventoryGiver.InventorySystem.InventorySlots[i].ItemData, 1);
                inventoryGiver.InventorySystem.InventorySlots[i].ClearSlot();
            }
        }
    }

    public List<ItemTypeAndCount> items = new List<ItemTypeAndCount>();


    //Method that gets all the items in the inventory and their count
    public List<ItemTypeAndCount> GetAllItems()
    {
        items.Clear();

        foreach (InventorySlot slot in inventorySystem.InventorySlots)
        {
                int i = 0;
                bool wasItemAdded = false;
                foreach (ItemTypeAndCount itemAndCount in items)
                {
                    if (itemAndCount.item == slot.ItemData)
                    {
                        items[i].count += 1;
                        wasItemAdded = true;
                    }

                    i++;
                }

                if (!wasItemAdded)
                {
                    items.Add(new ItemTypeAndCount(slot.ItemData, 1));
                }

        }
        Debug.Log(items);
        return items;
    }

}
