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
    Transfer = 1 << 2,
    Build = 1 << 3,
    CancelBuild = 1 << 4
}

public class BuildingActionSelection : MonoBehaviour
{

    private AssignWorkerInventory assignWorker;
    private BuildingManager buildingManager;
    public BuildingManager BuildingManager => buildingManager;
    private BuildingInventory buildingInventory;

    [SerializeField] private TextMeshProUGUI currentAmontOfWorkersText;
    [SerializeField] private TextMeshProUGUI MaxAmountOfWorkersText;

    public TransferType TypeOfTransfer;
    List<CharacterManager> charasFound = new List<CharacterManager>();


    [Header("Transfer Panel")]

    [SerializeField] private GameObject transferPanel;
    public GameObject TransferPanel => transferPanel; 


    private ResourceDropdownHandler dropdownHandler;
    public ResourceDropdownHandler DropdownHandler => dropdownHandler; 


    private TransferDropDown transferDropDownAdd;
    public TransferDropDown TransferDropDownADD => transferDropDownAdd;


    private TransferDropDown transferDropDownSub;
    public TransferDropDown TransferDropDownSUB => transferDropDownSub;


    private NumberInput amount;
    public NumberInput Amount => amount;


    [SerializeField]  private TMP_Dropdown resourceDropDown;
    public TMP_Dropdown ResourceDropDown => resourceDropDown;

    [SerializeField]  private TMP_Dropdown transferFromDropDown;
    public TMP_Dropdown TransferFromDropdown => transferFromDropDown;

    [SerializeField] private TMP_Dropdown transferToDropDown;
    public TMP_Dropdown TransferToDropdown => transferToDropDown;

    [SerializeField] private TMP_InputField inputAmount;
    public TMP_InputField InputAmount => inputAmount;


    private void Awake()
    {
        buildingInventory = gameObject.GetComponentInParent<BuildingInventory>();
        buildingManager = gameObject.GetComponentInParent<BuildingManager>();
        assignWorker = gameObject.GetComponentInParent<AssignWorkerInventory>();

        dropdownHandler = ResourceDropDown.gameObject.GetComponent<ResourceDropdownHandler>();
        transferDropDownAdd = TransferToDropdown.gameObject.GetComponent<TransferDropDown>();
        transferDropDownSub = TransferFromDropdown.gameObject.GetComponent<TransferDropDown>();
        amount = inputAmount.gameObject.GetComponent<NumberInput>();
        UpdateAssignedWorkerUI();

        transferPanel.SetActive(false);
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
        /*
        Add UI selection here
        */
        foreach (CharacterManager c in charasFound)
        {
            c.isTransferingItems = true;
        }
    }

    public void TakeItems()
    {
        TypeOfTransfer = TransferType.Take;
        GetReferences();
        /*
        Add UI selection here
        */
        foreach (CharacterManager c in charasFound) 
        {
            c.isTransferingItems = true;
        }
    }

    public void DisplayTransferWindow()
    {
    if(dropdownHandler !=null && transferDropDownAdd != null && transferDropDownSub != null)
        {
            if (!transferPanel.activeInHierarchy)
            {

                transferPanel.SetActive(true);
                dropdownHandler.UpdateDropdown();
                transferDropDownAdd.UpdateDropDown();
                transferDropDownSub.UpdateDropDown();
            }
            else transferPanel.SetActive(false);
        }
    }

    public void TransferItems()
    {
        TypeOfTransfer = TransferType.Transfer;
        GetReferences();

        foreach (CharacterManager c in charasFound)
        {
            c.isTransferingItems = true;
        }

    }

    public List<CharacterManager> GetReferences()
    {
        charasFound.Clear();
        foreach (UnitManager unit in Global.SELECTED_CHARACTERS)
        {
            CharacterManager characterManagerRef = unit.GetComponent<CharacterManager>();
            
            if (characterManagerRef != null)
            {
                charasFound.Add(characterManagerRef);

                WorkerAITransfer workerAITransferRef = characterManagerRef.GetComponentInChildren<WorkerAITransfer>();

                workerAITransferRef.ActionSelection = this;
                workerAITransferRef.TargetBuilding = buildingManager;
                workerAITransferRef.TargetInventory = buildingInventory;

            }
        }
        return charasFound;
    }

    public void StartBuilding()
    {
        assignWorker.AddWorkersToList();
    }

    public void CancelBuilding()
    {

    }

}
