using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AssignWorker : InventoryHolder
{
    public List<CharacterManager> WorkersAssigned = new List<CharacterManager>();

    private CharacterManager characterManager;
    private ItemRef workerItem;

    public void AddWorkersToList()
    {
        WorkersAssigned.Clear();
        foreach (UnitManager _worker in Global.SELECTED_UNITS) 
        {
            CharacterManager characterToAdd = _worker.gameObject.GetComponent<CharacterManager>();
            if ( characterToAdd != null)
            {

                WorkersAssigned.Add(characterToAdd);
            }
        }
        if(WorkersAssigned.Count <= inventorySystem.AmountOfSlotsAvaliable()) 
        {
            foreach(CharacterManager _worker in WorkersAssigned) 
            {
                if(_worker.isAssignedToABuilding == false)
                {
                    _worker.isAssignedToABuilding = true;
                    workerItem = _worker.gameObject.GetComponent<ItemRef>();
                    inventorySystem.AddToInventory(workerItem.item, 1);
                }
            }

        }


            

    }


}
