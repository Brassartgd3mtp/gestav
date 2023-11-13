using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class AssignWorker : InventoryHolder
{
    public List<CharacterManager> workersFound = new List<CharacterManager>(); //contain the characters that are currently selected

    private CharacterManager characterManager;
    private ItemRef workerItem;
    public BuildingManager buildingManager;
    private BuildingActionSelection buildingActionSelection;

    public void AddWorkersToList()
    {
        buildingManager = this.gameObject.GetComponent<BuildingManager>();
        buildingActionSelection = this.gameObject.GetComponentInChildren<BuildingActionSelection>();

        FindWorkers();

        if(workersFound.Count <= inventorySystem.AmountOfSlotsAvaliable()) 
        {
            foreach(CharacterManager _worker in workersFound) 
            {
                if(_worker.isAssignedToABuilding == false)
                {
                    _worker.isAssignedToABuilding = true;
                    workerItem = _worker.gameObject.GetComponent<ItemRef>();

                    _worker.buildingAssigned = buildingManager;

                    inventorySystem.AddToInventory(workerItem.item, 1);
                    buildingActionSelection.UpdateAssignedWorkerUI();
                }
            }

        }


            

    }

    public void RemoveWorkers()
    {
        FindWorkers();

        int amountOfWorkersToRemove = 0;

        foreach (CharacterManager _worker in workersFound)
            {
                if (_worker.isAssignedToABuilding == true && _worker.buildingAssigned == buildingManager)
                {

                    amountOfWorkersToRemove++;
                    _worker.isAssignedToABuilding = false;
                    _worker.buildingAssigned = null;


                    
                }
            }
        Debug.Log(amountOfWorkersToRemove);

        foreach (InventorySlot slot in InventorySystem.InventorySlots)
        {
            if(amountOfWorkersToRemove > 0)
            {
                slot.ClearSlot();
                amountOfWorkersToRemove--;
            }
        }


        buildingActionSelection.UpdateAssignedWorkerUI();
    }

    public List<CharacterManager> FindWorkers()
    {
        workersFound.Clear();
        foreach (UnitManager _worker in Global.SELECTED_UNITS)
        {
            CharacterManager characterToAdd = _worker.gameObject.GetComponent<CharacterManager>();
            if (characterToAdd != null)
            {
                workersFound.Add(characterToAdd);
            }
        }

        return workersFound;
    }

}
