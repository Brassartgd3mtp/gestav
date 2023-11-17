using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WorkerAIC;
using System.Threading.Tasks;

public class WorkerAITransfer : WorkerBehaviour
{
    public BuildingManager TargetBuilding;
    public BuildingInventory TargetInventory;
    private UnitInventory inventory;
    public BuildingActionSelection ActionSelection;

    List<ItemTypeAndCount> items = new List<ItemTypeAndCount>();

    private bool buildingReached;

    protected override void Awake()
    {
        base.Awake();
        buildingReached = false;
        inventory = gameObject.GetComponentInParent<UnitInventory>();
        canBeMovedbyPlayer = false;
        TransferStarted = false;
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
        if (!buildingReached && !TransferStarted)
        {
            GoToBuilding();
        }
        else
        {
            switch (ActionSelection.TypeOfTransfer)
            {
                case TransferType.Deposit:
                    DoDeposit();
                    break;
                case TransferType.Take:
                    DoTake();
                    break;
                case TransferType.Transfer:
                    DoTransfer();
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
        if (inventory.InventorySystem.AmountOfSlotsAvaliable() >= 0)
        {
            for (int i = 0; i < inventory.InventorySystem.InventorySlots.Count; i++)
            {

                if (inventory.InventorySystem.InventorySlots[i].ItemData == null)
                {
                    int lastSlot = TargetInventory.InventorySystem.InventorySize - (TargetInventory.InventorySystem.AmountOfSlotsAvaliable() + 1);
                    if (lastSlot < 0) lastSlot = 0;

                    if (TargetInventory.InventorySystem.InventorySlots[lastSlot].ItemData != null)
                    {
                        inventory.InventorySystem.AddToInventory(TargetInventory.InventorySystem.InventorySlots[lastSlot].ItemData, 1);
                        TargetInventory.InventorySystem.InventorySlots[lastSlot].ClearSlot();
                    }
                }
            }
            CharacterManagerRef.ShowBag();
        }

        buildingReached = false;
        CharacterManagerRef.isTransferingItems = false;
    }

    public async void DoTransfer()
    {
        InventoryItemData resourceToTransfer = ActionSelection.DropdownHandler.CurrentlyAssociatedData;
        BuildingInventory inventoryToTakeFrom = ActionSelection.TransferDropDownSUB.CurrentlyAssociatedData;
        BuildingInventory inventoryToTakeTo = ActionSelection.TransferDropDownADD.CurrentlyAssociatedData;
        int amountToTransfer = ActionSelection.Amount.Result;

        items = inventoryToTakeFrom.GetAllItems();
        int amountAvaliable = 0;

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].item == resourceToTransfer)
            {
                amountAvaliable = items[i].count;
                Debug.Log("item trouvé");
                break;
            }
        }
        if (amountAvaliable >= amountToTransfer && inventoryToTakeTo.InventorySystem.AmountOfSlotsAvaliable() >= amountToTransfer)
        {
            /* TransferStarted = true;
               TargetBuilding = inventoryToTakeFrom.GetComponent<BuildingManager>();
               Debug.Log("va prendre");
               buildingReached = false;
               GoToBuilding();
               StartCoroutine(WaitForBuildingReached());
               TargetInventory = inventoryToTakeFrom;
               Debug.Log("prend");
               DoTake();
               TargetBuilding = inventoryToTakeTo.GetComponent<BuildingManager>();
               Debug.Log("va poser");
               buildingReached = false;
               GoToBuilding();
               StartCoroutine(WaitForBuildingReached());
               TargetInventory = inventoryToTakeTo;
               Debug.Log("pose");
               DoDeposit();
               Debug.Log("transfert ok");
               TransferStarted = false; */

            TransferStarted = true;
            TargetBuilding = inventoryToTakeFrom.GetComponent<BuildingManager>();
            Debug.Log("va prendre");


            buildingReached = false;
            Transform targetTransform = TargetBuilding.gameObject.transform;
            float distanceToStop = targetTransform.GetComponent<BoxCollider>().size.z + 1.5f;
            TargetInventory = inventoryToTakeFrom;
            Debug.Log("prend");
            CharacterManagerRef.MoveTo(targetTransform.position, distanceToStop);

            while (!buildingReached)
            {
                if (Vector3.Distance(transform.position, targetTransform.position) < distanceToStop)
                {
                    buildingReached = true;
                    break;
                }
                await Task.Delay(250);
            }

            TargetInventory = inventoryToTakeFrom;

            if (inventory.InventorySystem.AmountOfSlotsAvaliable() >= 0)
            {
                Debug.Log("slotsavaliable ok");
                for (int i = 0; i < inventory.InventorySystem.InventorySlots.Count; i++)
                {

                    if (inventory.InventorySystem.InventorySlots[i].ItemData == null)
                    {
                        Debug.Log("slot libre");
                        int lastSlot = TargetInventory.InventorySystem.InventorySize - (TargetInventory.InventorySystem.AmountOfSlotsAvaliable() + 1);
                        if (lastSlot < 0) lastSlot = 0;

                        if (TargetInventory.InventorySystem.InventorySlots[lastSlot].ItemData != null)
                        {
                            Debug.Log("item trouvé dans l'inventaire");
                            inventory.InventorySystem.AddToInventory(TargetInventory.InventorySystem.InventorySlots[lastSlot].ItemData, 1);
                            TargetInventory.InventorySystem.InventorySlots[lastSlot].ClearSlot();
                        }
                    }
                }
                CharacterManagerRef.ShowBag();
            }




            TargetBuilding = inventoryToTakeTo.GetComponent<BuildingManager>();
            buildingReached = false;

            Debug.Log("va poser");
            targetTransform = TargetBuilding.gameObject.transform;
            distanceToStop = targetTransform.GetComponent<BoxCollider>().size.z + 1.5f;

            CharacterManagerRef.MoveTo(targetTransform.position, distanceToStop);

            while (!buildingReached)
            {
                if (Vector3.Distance(transform.position, targetTransform.position) < distanceToStop)
                {
                    buildingReached = true;
                    break;
                }
                await Task.Delay(250);
            }

            TargetInventory = inventoryToTakeTo;

            Debug.Log("pose");
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
            Debug.Log("transfert ok");
            TransferStarted = false;
        }
        else
        {
            Debug.Log("transfert pas ok");
            CharacterManagerRef.isTransferingItems = false;
            buildingReached = false;
            TransferStarted = false;
        }

        CharacterManagerRef.isTransferingItems = false;
    }
    private IEnumerator WaitForBuildingReached()
    {
        yield return new WaitForSeconds(0.5f);
    }
}
