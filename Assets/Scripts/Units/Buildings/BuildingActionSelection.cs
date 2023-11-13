using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using System;

[Flags]
public enum TransferType
{
    Deposit = 1 << 0,
    Take = 1 << 1,
    Transfer = 1 << 2
}

public class BuildingActionSelection : MonoBehaviour
{

    private CharacterManager characterManager;
    private AssignWorker assignWorker;
    private BuildingManager buildingManager;
    public BuildingManager BuildingManager => buildingManager;
    private BuildingInventory buildingInventory;

    [SerializeField] private TextMeshProUGUI currentAmontOfWorkersText;
    [SerializeField] private TextMeshProUGUI MaxAmountOfWorkersText;

    public TransferType TypeOfTransfer;


    private void Awake()
    {
        buildingInventory = gameObject.GetComponentInParent<BuildingInventory>();
        buildingManager = gameObject.GetComponentInParent<BuildingManager>();
        assignWorker = gameObject.GetComponentInParent<AssignWorker>();
        UpdateAssignedWorkerUI();
    }
    public void UseBuilding()
    {
        assignWorker.AddWorkersToList();
    }

    public void UpdateAssignedWorkerUI()
    {
        currentAmontOfWorkersText.text = (assignWorker.InventorySystem.InventorySize - assignWorker.InventorySystem.AmountOfSlotsAvaliable()).ToString();
        MaxAmountOfWorkersText.text = assignWorker.InventorySystem.InventorySize.ToString();
    }
    
    public void DepositItems() 
    {
        TypeOfTransfer = TransferType.Deposit;
        GetReferences();
    }

    public void TakeItems()
    {
        TypeOfTransfer = TransferType.Take;
        GetReferences();
    }

    public void TransferItems()
    {
        TypeOfTransfer = TransferType.Transfer;
        GetReferences();
    }

    public void GetReferences()
    {
        foreach (UnitManager unit in Global.SELECTED_UNITS)
        {

            CharacterManager characterManagerRef = unit.GetComponent<CharacterManager>();
            
            if (characterManagerRef != null)
            {
                WorkerAITransfer workerAITransferRef = characterManagerRef.GetComponentInChildren<WorkerAITransfer>();

                workerAITransferRef.ActionSelection = this;
                workerAITransferRef.TargetBuilding = buildingManager;
                workerAITransferRef.TargetInventory = buildingInventory;
                characterManagerRef.isTransferingItems = true;
            }
        }
    }

}
