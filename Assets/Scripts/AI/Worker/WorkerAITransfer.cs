using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WorkerAIC;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;

public class WorkerAITransfer : WorkerBehaviour
{
    public BuildingManager TargetBuilding;
    public BuildingInventory TargetInventory;
    private UnitInventory inventory;
    public BuildingActionSelection ActionSelection;
    private BuildingStockageUI buildingStockageUI;

    List<ItemTypeAndCount> items = new List<ItemTypeAndCount>();

    private bool buildingReached;

    protected override void Awake()
    {
        base.Awake();
        buildingReached = false;
        inventory = gameObject.GetComponentInParent<UnitInventory>();
        canBeMovedbyPlayer = false;
        TransferStarted = false;
        buildingStockageUI = null;
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
        buildingStockageUI = TargetInventory.gameObject.GetComponent<BuildingStockageUI>();

        for (int i = 0; i < inventory.InventorySystem.InventorySlots.Count; i++)
        {

            if (inventory.InventorySystem.InventorySlots[i].ItemData != null && TargetInventory.validType.Contains(inventory.InventorySystem.InventorySlots[i].ItemData.resourceType))
            {
                TargetInventory.InventorySystem.AddToInventory(inventory.InventorySystem.InventorySlots[i].ItemData, 1);
                inventory.InventorySystem.InventorySlots[i].ClearSlot();
                buildingStockageUI.UpdateSpaceInUI();
            }
        }
        CharacterManagerRef.HideBag();

        buildingReached = false;
        CharacterManagerRef.isTransferingItems = false;
    }

    public void DoTake()
    {
        buildingStockageUI = TargetInventory.gameObject.GetComponent<BuildingStockageUI>();

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
                        buildingStockageUI.UpdateSpaceInUI();
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

        Debug.Log("take from " + inventoryToTakeFrom);
        Debug.Log("take to " + inventoryToTakeTo);

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

        Debug.Log("amount avaliable :" + amountAvaliable);

        // transfer from a building to another

        if(inventoryToTakeTo.gameObject.GetComponent<Extractioninventory>() != null)
        {
            Extractioninventory extractionInventory;

            if(!TransferStarted) 
            {
                TransferStarted = true;
                currentActionText.text = "Transfering Items . . .";
                currentActionText.outlineWidth = 0.35f;
                currentActionText.color = Color.white;
                currentActionText.outlineColor = Color.black;

                for (int amountTransfered = 0; amountTransfered < amountToTransfer; amountTransfered += 0)
                {
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
                        }
                        await Task.Delay(500);
                    }

                    TargetInventory = inventoryToTakeFrom;
                    buildingStockageUI = TargetInventory.gameObject.GetComponent<BuildingStockageUI>();
                    Debug.Log(buildingReached);
                    if (buildingReached)
                    {
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

                                    if (TargetInventory.InventorySystem.InventorySlots[lastSlot].ItemData != null && amountTransfered + i < amountToTransfer)
                                    {
                                        Debug.Log("item trouvé dans l'inventaire");
                                        inventory.InventorySystem.AddToInventory(TargetInventory.InventorySystem.InventorySlots[lastSlot].ItemData, 1);
                                        TargetInventory.InventorySystem.InventorySlots[lastSlot].ClearSlot();
                                        buildingStockageUI.UpdateSpaceInUI();
                                    }
                                }
                            }
                            CharacterManagerRef.ShowBag();
                        }
                    }


                    buildingReached = false;

                    Debug.Log("va poser"); 
                    targetTransform = inventoryToTakeTo.gameObject.GetComponent<Transform>();
                    distanceToStop = targetTransform.GetComponent<BoxCollider>().size.z + 5f;

                    CharacterManagerRef.MoveTo(targetTransform.position, distanceToStop);

                    while (!buildingReached)
                    {
                        Debug.Log("while");
                        if (Vector3.Distance(transform.position, targetTransform.position) < distanceToStop)
                        {
                            Debug.Log("reached");
                            buildingReached = true;
                        }
                        await Task.Delay(500);
                        Debug.Log("Pas reach");
                    }

                    TargetInventory = inventoryToTakeTo;
                    buildingStockageUI = TargetInventory.gameObject.GetComponent<BuildingStockageUI>();
                    extractionInventory = TargetInventory.gameObject.GetComponent<Extractioninventory>(); 

                    if (buildingReached)
                    {
                        Debug.Log("pose");
                        for (int i = 0; i < inventory.InventorySystem.InventorySlots.Count; i++)
                        {

                            if (inventory.InventorySystem.InventorySlots[i].ItemData != null && TargetInventory.validType.Contains(inventory.InventorySystem.InventorySlots[i].ItemData.resourceType) && amountTransfered < amountToTransfer)
                            {
                                TargetInventory.InventorySystem.InventorySlots.Add(new InventorySlot());
                                TargetInventory.InventorySystem.AddToInventory(inventory.InventorySystem.InventorySlots[i].ItemData, 1);
                                inventory.InventorySystem.InventorySlots[i].ClearSlot();
                                amountTransfered++;
                                extractionInventory.UpdateInventoryInfo();
                            }
                        }

                        CharacterManagerRef.HideBag();
                        buildingReached = false;
                    }
                }


                Debug.Log("transfer done");
                TransferStarted = false;
                CharacterManagerRef.isTransferingItems = false;

            }
            else
            {
                Debug.Log("transfert pas ok");
                CharacterManagerRef.isTransferingItems = false;
                buildingReached = false;
                TransferStarted = false;
            }
        }
        else
        {
            if (amountAvaliable >= amountToTransfer && inventoryToTakeTo.InventorySystem.AmountOfSlotsAvaliable() >= amountToTransfer && !TransferStarted)
            {
                TransferStarted = true;
                currentActionText.text = "Transfering Items . . .";
                currentActionText.outlineWidth = 0.35f;
                currentActionText.color = Color.white;
                currentActionText.outlineColor = Color.black;

                for (int amountTransfered = 0; amountTransfered < amountToTransfer; amountTransfered += 0)
                {
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
                        }
                        await Task.Delay(500);
                    }


                    TargetInventory = inventoryToTakeFrom;
                    buildingStockageUI = TargetInventory.gameObject.GetComponent<BuildingStockageUI>();
                    Debug.Log(buildingReached);
                    if (buildingReached)
                    {
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



                                    if (TargetInventory.InventorySystem.InventorySlots[lastSlot].ItemData != null && amountTransfered + i < amountToTransfer)
                                    {
                                        Debug.Log("item trouvé dans l'inventaire");
                                        inventory.InventorySystem.AddToInventory(TargetInventory.InventorySystem.InventorySlots[lastSlot].ItemData, 1);
                                        TargetInventory.InventorySystem.InventorySlots[lastSlot].ClearSlot();
                                        buildingStockageUI.UpdateSpaceInUI();
                                    }
                                }
                            }
                            CharacterManagerRef.ShowBag();
                        }
                    }


                    TargetBuilding = inventoryToTakeTo.GetComponent<BuildingManager>();
                    buildingReached = false;

                    Debug.Log("va poser");
                    targetTransform = TargetBuilding.gameObject.transform;
                    distanceToStop = targetTransform.GetComponent<BoxCollider>().size.z + 1.5f;

                    CharacterManagerRef.MoveTo(targetTransform.position, distanceToStop);

                    while (!buildingReached)
                    {
                        Debug.Log("while");
                        if (Vector3.Distance(transform.position, targetTransform.position) < distanceToStop)
                        {
                            Debug.Log("reached");
                            buildingReached = true;
                        }
                        await Task.Delay(500);
                        Debug.Log("Pas reach");
                    }

                    TargetInventory = inventoryToTakeTo;
                    buildingStockageUI = TargetInventory.gameObject.GetComponent<BuildingStockageUI>();

                    if (buildingReached)
                    {
                        Debug.Log("pose");
                        for (int i = 0; i < inventory.InventorySystem.InventorySlots.Count; i++)
                        {

                            if (inventory.InventorySystem.InventorySlots[i].ItemData != null && TargetInventory.validType.Contains(inventory.InventorySystem.InventorySlots[i].ItemData.resourceType) && amountTransfered < amountToTransfer)
                            {
                                TargetInventory.InventorySystem.AddToInventory(inventory.InventorySystem.InventorySlots[i].ItemData, 1);
                                inventory.InventorySystem.InventorySlots[i].ClearSlot();
                                amountTransfered++;
                                buildingStockageUI.UpdateSpaceInUI();
                            }
                        }

                        CharacterManagerRef.HideBag();
                        buildingReached = false;
                    }
                }


                Debug.Log("transfer done");
                TransferStarted = false;
                CharacterManagerRef.isTransferingItems = false;
            }
            else
            {
                Debug.Log("transfert pas ok");
                CharacterManagerRef.isTransferingItems = false;
                buildingReached = false;
                TransferStarted = false;
            }
        }
    }
}

