using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class BuildingActionSelection : MonoBehaviour
{
    private CharacterManager characterManager;
    private AssignWorker assignWorker;
    [SerializeField] private TextMeshProUGUI currentAmontOfWorkersText;
    [SerializeField] private TextMeshProUGUI MaxAmountOfWorkersText;

    private void Awake()
    {
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

    
}
