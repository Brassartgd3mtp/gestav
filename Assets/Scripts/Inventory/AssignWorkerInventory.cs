using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class AssignWorkerInventory : InventoryHolder
{
    public List<CharacterManager> workersFound = new List<CharacterManager>(); //contain the characters that are currently selected

    private ItemRef workerItem;
    public BuildingManager buildingManager;
    private BuildingActionSelection buildingActionSelection;
    public ResourceSpotUI resourceSpot;
    public void AddWorkersToList()
    {
        buildingManager = this.gameObject.GetComponent<BuildingManager>();
        buildingActionSelection = this.gameObject.GetComponentInChildren<BuildingActionSelection>();
        resourceSpot = this.gameObject.GetComponent<ResourceSpotUI>();

        FindWorkers();

        if(workersFound.Count <= inventorySystem.AmountOfSlotsAvaliable()) 
        {
            foreach(CharacterManager _worker in workersFound) 
            {
                if(!_worker.isAssignedToABuilding)
                {
                    _worker.isAssignedToABuilding = true;
                    workerItem = _worker.gameObject.GetComponent<ItemRef>();

                    if(buildingManager != null )
                    {
                        _worker.buildingAssigned = buildingManager;
                    }
                    else if(resourceSpot != null)
                    {
                        _worker.resourceAssigned = resourceSpot;
                    }


                    inventorySystem.AddToInventory(workerItem.item, 1);
                    if(buildingActionSelection != null)
                    {
                        buildingActionSelection.UpdateAssignedWorkerUI();
                    }

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
                if (_worker.isAssignedToABuilding == true)
                {

                    if (_worker.buildingAssigned != null && _worker.buildingAssigned == buildingManager)
                    {   
                        amountOfWorkersToRemove++;
                        _worker.isAssignedToABuilding = false;
                        _worker.buildingAssigned = null;
                    }
                    if (_worker.resourceAssigned != null && _worker.resourceAssigned == resourceSpot)
                    {
                        amountOfWorkersToRemove++;
                        _worker.isAssignedToABuilding = false;
                        _worker.resourceAssigned = null;
                    }

    
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
        foreach (UnitManager _worker in Global.SELECTED_CHARACTERS)
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
