using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnitInventory : InventoryHolder, IInteractable
{

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
            if(inventoryGiver.InventorySystem.InventorySlots[i].ItemData.resourceType == inventoryReceiver.validType)
            {
                inventoryReceiver.InventorySystem.AddToInventory(inventoryGiver.InventorySystem.InventorySlots[i].ItemData, 1);
                inventoryGiver.InventorySystem.InventorySlots[i].ClearSlot();
            }
        }
    }

}
