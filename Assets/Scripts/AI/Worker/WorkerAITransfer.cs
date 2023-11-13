using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WorkerAIC;

public class WorkerAITransfer : WorkerBehaviour
{
    public BuildingManager TargetBuilding;
    public BuildingInventory TargetInventory;
    private UnitInventory inventory;
    public BuildingActionSelection ActionSelection;

    private bool buildingReached;

    protected override void Awake()
    {
        base.Awake();
        buildingReached = false;
        inventory = gameObject.GetComponentInParent<UnitInventory>();
        canBeMovedbyPlayer = false;

    }
    public override void ApplyBehaviour()
    {
        if (CharacterManagerRef.isTransferingItems)
        {
            DoTransferActions();
        }
    }
    public override BehaviourName CheckTransition()
    {
        if (!CharacterManagerRef.isTransferingItems)
        {
            return BehaviourName.Controlled;
        }
        return BehaviourName.Transfer;
    }

    public void DoTransferActions()
    {
        if (!buildingReached)
        {
            GoToBuilding();
        }
        else
        {
            switch (ActionSelection.TypeOfTransfer)
            {
                case TransferType.Deposit :
                    DoDeposit();
                    break;
                case TransferType.Take:
                    DoTake();
                    break;
                case TransferType.Transfer:
                    break;
            }
        }
    }

        public void GoToBuilding()
        {
            buildingReached = false;
            Transform targetTransform = TargetBuilding.gameObject.transform;
            float distanceToStop = targetTransform.GetComponent<BoxCollider>().size.z + 1.5f;

            CharacterManagerRef.MoveTo(targetTransform.position, distanceToStop);
            if (Vector3.Distance(transform.position, targetTransform.position) < distanceToStop)
            {
                buildingReached = true;
            }
        }

        public void DoDeposit()
        {
            for (int i = 0; i < inventory.InventorySystem.InventorySlots.Count; i++)
            {

            if (inventory.InventorySystem.InventorySlots[i].ItemData != null && inventory.InventorySystem.InventorySlots[i].ItemData.resourceType == TargetInventory.validType)
                {
                    TargetInventory.InventorySystem.AddToInventory(inventory.InventorySystem.InventorySlots[i].ItemData, 1);
                    inventory.InventorySystem.InventorySlots[i].ClearSlot();
                }
            }
                CharacterManagerRef.HideBag();

            buildingReached = false;
            CharacterManagerRef.isTransferingItems = false;
        }

    public void DoTake()
    {
        for (int i = 0; i < inventory.InventorySystem.InventorySlots.Count; i++)
        {

            if (inventory.InventorySystem.InventorySlots[i].ItemData == null)
            {
                int lastSlot = TargetInventory.InventorySystem.InventorySize - TargetInventory.InventorySystem.AmountOfSlotsAvaliable();
                inventory.InventorySystem.AddToInventory(TargetInventory.InventorySystem.InventorySlots[lastSlot].ItemData, 1);
                TargetInventory.InventorySystem.InventorySlots[lastSlot].ClearSlot();
            }
        }
        CharacterManagerRef.ShowBag();

        CharacterManagerRef.isTransferingItems = false;
    }

    public void DoTransfer()
    {

    }

}
