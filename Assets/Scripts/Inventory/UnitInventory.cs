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


}
