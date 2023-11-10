using System.Collections;
using System.Collections.Generic;
using System.Resources;
using TMPro;
using UnityEngine;
using static UnityEditor.Progress;
using static WorkerAIC;
using System.Threading.Tasks;

public class WorkerAIUse : WorkerBehaviour
{
    private bool buildingReached;
    private Find findingScript;
    private float timer;

    protected override void Awake()
    {
        base.Awake();
        findingScript = GetComponentInParent<Find>();
        canBeMovedbyPlayer = false;
    }

    public override void ApplyBehaviour()
    {
        if (CharacterManagerRef.isAssignedToABuilding)
        {
            UseBuilding();
        }
    }
    public override BehaviourName CheckTransition()
    {
        if (!CharacterManagerRef.isAssignedToABuilding)
        {
            return BehaviourName.Wait;
        }

        return BehaviourName.Use;
    }

    public void UseBuilding()
    {

        if (!buildingReached)
        {
            GoToUsedBuilding();
        }
        else
        {
            switch (CharacterManagerRef.buildingAssigned.TypeOfBuilding)
            {
                case BuildingType.Collect:
                    if (CharacterManagerRef.GatheringMode)
                    {
                        GoMining();
                    }
                    else
                    {
                        GoStoreResources();
                    }



                    break;
                case BuildingType.Crafting:
                    //
                    break;
            }
        }
    }

    public void GoToUsedBuilding()
    {
        Debug.Log("Going to building");
        buildingReached = false;
        Transform targetTransform = CharacterManagerRef.buildingAssigned.gameObject.transform;
        float distanceToStop = targetTransform.GetComponent<BoxCollider>().size.z + 1.5f;

        CharacterManagerRef.MoveTo(targetTransform.position, distanceToStop);
        if (Vector3.Distance(transform.position, targetTransform.position) < distanceToStop)
        {
            buildingReached = true;
            CharacterManagerRef.EnterGatheringMode();
        }

    }

    public void GoMining()
    {
        Debug.Log("Going To Mine");
        //search all the resources 
        Transform[] resourcesTranform = findingScript.GetTransformArray(Global.RESOURCE_LAYER_MASK);

        ItemRef[] resourcesTypes = new ItemRef[resourcesTranform.Length];
        List<ItemRef> correspondingItems = new List<ItemRef>();
        BuildingInventory buildinv = CharacterManagerRef.buildingAssigned.GetComponent<BuildingInventory>();


        //add resources with the same type as the building to a list
        for (int i = 0; i < resourcesTranform.Length; i++)
        {
            resourcesTypes[i] = resourcesTranform[i].gameObject.GetComponentInChildren<ItemRef>();
        }
        foreach (ItemRef item in resourcesTypes)
        {
            if (item != null && item.item.resourceType == buildinv.validType)
            {
                correspondingItems.Add(item);
            }
        }
        Transform targetTransform = GetClosestResource(correspondingItems); // Trouve la mine la plus proche
        if (targetTransform != null) //Si a trouv� une ressource
        {
            float distanceToStop = targetTransform.GetComponentInParent<BoxCollider>().size.z + 1.5f;
            Vector3 targetLocation = targetTransform.position;
            CharacterManagerRef.MoveTo(targetLocation, distanceToStop); //Va a la position de la mine
        }
    }

    public Transform GetClosestResource(List<ItemRef> correspondingItems)
    {
        Debug.Log("CheckingForClosestResource");
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (ItemRef item in correspondingItems)
        {
            float dist = Vector3.Distance(item.gameObject.transform.position, currentPos);
            if (dist < minDist)
            {
                tMin = item.gameObject.transform;
                minDist = dist;
            }
        }
        return tMin;
    }


    public async void GoStoreResources() // The method that tell the worker to go store the resources he gathered in a building
    {
        Debug.Log("Going to storage");
        CharacterManagerRef.ExitGatheringMode();

        Transform targetTransform = CharacterManagerRef.buildingAssigned.gameObject.transform;
        Debug.Log(targetTransform.position);

        if (targetTransform != null) //Si a trouv� le batiment
        {
            float distanceToStop = targetTransform.GetComponent<BoxCollider>().size.z + 1.5f;
            bool locationReached = false;
            Vector3 targetLocation = targetTransform.position;
            CharacterManagerRef.MoveTo(targetLocation, distanceToStop); //Va a la position de la mine

            while (locationReached == false)
            {
                if (Vector3.Distance(transform.position, targetLocation) <= distanceToStop) //Si l'ouvrier est suffisament pr�s de la mine
                {
                    Debug.Log("DESTINATION REACHED");
                    InventoryHolder _buildInv = targetTransform.GetComponent<InventoryHolder>();
                    for (int i = 0; i < CharacterManagerRef.Inventory.InventorySystem.InventorySlots.Count; i++) //transfert les objets de son inventaire � celui de la mine
                    {

                        //Put worker items into the building
                        Debug.Log(CharacterManagerRef.Inventory.InventorySystem.InventorySlots[i].ItemData);

                        if (CharacterManagerRef.Inventory.InventorySystem.InventorySlots[i].ItemData != null && CharacterManagerRef.Inventory.InventorySystem.InventorySlots[i].ItemData.resourceType == _buildInv.validType)
                        {
                            _buildInv.InventorySystem.AddToInventory(CharacterManagerRef.Inventory.InventorySystem.InventorySlots[i].ItemData, 1);
                            CharacterManagerRef.Inventory.InventorySystem.InventorySlots[i].ClearSlot();

                            CharacterManagerRef.ChangeBagSize(CharacterManagerRef.CalculateBagSize()); // change the bag size

                            await Task.Delay(CharacterManagerRef.DepositDuration);

                            //change the UI pop up on top of the building
                            BuildingStockageUI buildingStockageUI = _buildInv.gameObject.GetComponent<BuildingStockageUI>();
                            if (buildingStockageUI != null)
                            {
                                buildingStockageUI.UpdateSpaceInUI();
                            }

                            //Dynamic display of character inventory if he is selected
                            CharacterManagerRef.DisplayThisIventory();

                            //Dynamic display of building inventory if it is selected
                            if (Global.SELECTED_UNITS.Count == 1 && Global.SELECTED_UNITS[0] == _buildInv.gameObject.GetComponent<UnitManager>())
                            {
                                CharacterManagerRef.ShowInventoryUI(_buildInv.InventorySystem);
                            }

                        }

                    }
                    _buildInv = null;
                    locationReached = true;

                    // If inventory is empty go mine, else find the new destination to empty his inventory
                    if (CharacterManagerRef.Inventory.InventorySystem.KnowIfInventoryIsEmpty())
                    {
                        Debug.Log("Inventory empty");
                        CharacterManagerRef.HideBag();
                        CharacterManagerRef.EnterGatheringMode();
                    }
                    else
                        Debug.Log("Inventory not empty");
                    GoDepositRemainingResources();


                }
                await Task.Delay(250);
            }

        }
    }

    public void GoDepositRemainingResources()
    {

    }
}
