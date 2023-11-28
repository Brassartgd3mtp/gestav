using System.Collections;
using System.Collections.Generic;
using System.Resources;
using TMPro;
using UnityEngine;
using static UnityEditor.Progress;
using static WorkerAIC;
using System.Threading.Tasks;
using Unity.VisualScripting;
using System.Xml;

public class WorkerAIUse : WorkerBehaviour
{
    private bool buildingReached;
    private Find findingScript;

    protected override void Awake()
    {
        base.Awake();
        findingScript = GetComponentInParent<Find>();
        canBeMovedbyPlayer = false;
    }
    public override void ApplyBehaviour()
    {
        if (CharacterManagerRef.isAssignedToABuilding && CharacterManagerRef.buildingAssigned.hasBeenBuilt)
        {
            UseBuilding();
        }
        else if(CharacterManagerRef.isAssignedToABuilding && !CharacterManagerRef.buildingAssigned.hasBeenBuilt)
        {
            DoBuild();
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
        currentActionText.text = "Collecting Items . . .";
        currentActionText.outlineColor = Color.black;
        currentActionText.color = Color.white;
        currentActionText.outlineWidth = 0.35f;

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
            if (item != null && buildinv.validType.Contains(item.item.resourceType))
            {
                correspondingItems.Add(item);
            }
        }
        Transform targetTransform = GetClosestResource(correspondingItems); // Trouve la mine la plus proche
        if (targetTransform != null) //Si a trouvé une ressource
        {
            float distanceToStop = targetTransform.GetComponentInParent<BoxCollider>().size.z + 1.5f;
            Vector3 targetLocation = targetTransform.position;
            CharacterManagerRef.MoveTo(targetLocation, distanceToStop); //Va a la position de la mine
        }
    }

    public Transform GetClosestResource(List<ItemRef> correspondingItems)
    {
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
        CharacterManagerRef.ExitGatheringMode();

        Transform targetTransform = CharacterManagerRef.buildingAssigned.gameObject.transform;

        if (targetTransform != null) //Si a trouvé le batiment
        {
            float distanceToStop = targetTransform.GetComponent<BoxCollider>().size.z + 1.5f;
            bool locationReached = false;
            Vector3 targetLocation = targetTransform.position;
            CharacterManagerRef.MoveTo(targetLocation, distanceToStop); //Va a la position de la mine

            while (locationReached == false)
            {
                if (Vector3.Distance(transform.position, targetLocation) <= distanceToStop) //Si l'ouvrier est suffisament près de la mine
                {
                    BuildingInventory _buildInv = targetTransform.GetComponent<BuildingInventory>();
                    for (int i = 0; i < CharacterManagerRef.Inventory.InventorySystem.InventorySlots.Count; i++) //transfert les objets de son inventaire à celui de la mine
                    {

                        //Put worker items into the building

                        if (CharacterManagerRef.Inventory.InventorySystem.InventorySlots[i].ItemData != null && _buildInv.validType.Contains(CharacterManagerRef.Inventory.InventorySystem.InventorySlots[i].ItemData.resourceType))
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
                            if (Global.SELECTED_BUILDINGS.Count == 1 && Global.SELECTED_BUILDINGS[0] == _buildInv.gameObject.GetComponent<UnitManager>())
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
                        CharacterManagerRef.HideBag();
                        CharacterManagerRef.EnterGatheringMode();
                    }
                    else
                    {

                    }



                }
                await Task.Delay(250);
            }

        }
    }

    public async void DoBuild()
    {
        Transform targetTransform = CharacterManagerRef.buildingAssigned.transform;
        ConstructionInventory constructionInventory = CharacterManagerRef.buildingAssigned.GetComponent<ConstructionInventory>();

        //SETP ONE : Check if assigned worker has resources to build in his inventory

        bool hasResourcesToBuild = false;

        for (int i = 0; i < CharacterManagerRef.Inventory.InventorySystem.InventorySlots.Count; i++)
        {
            if (constructionInventory.validType.Contains(CharacterManagerRef.Inventory.InventorySystem.InventorySlots[i].ItemData.resourceType))
            {
                hasResourcesToBuild = true;
                break;
            }

        }

        // STEP TWO : If worker has resources to build, go deposit them in the temporary building inventory

        if (hasResourcesToBuild)
        {
            if (targetTransform != null) //Si a trouvé le batiment
            {
                float distanceToStop = targetTransform.GetComponent<BoxCollider>().size.z + 1.5f;
                bool locationReached = false;
                Vector3 targetLocation = targetTransform.position;
                CharacterManagerRef.MoveTo(targetLocation, distanceToStop); //Va a la position du building

                while (locationReached == false)
                {
                    if (Vector3.Distance(transform.position, targetLocation) <= distanceToStop) //Si l'ouvrier est suffisament près de la mine
                    {
                        for (int i = 0; i < CharacterManagerRef.Inventory.InventorySystem.InventorySlots.Count; i++) //transfert les objets de son inventaire à celui de la mine
                        {

                            //Put worker items into the building

                            if (CharacterManagerRef.Inventory.InventorySystem.InventorySlots[i].ItemData != null && constructionInventory.validType.Contains(CharacterManagerRef.Inventory.InventorySystem.InventorySlots[i].ItemData.resourceType))
                            {
                                constructionInventory.InventorySystem.AddToInventory(CharacterManagerRef.Inventory.InventorySystem.InventorySlots[i].ItemData, 1);
                                CharacterManagerRef.Inventory.InventorySystem.InventorySlots[i].ClearSlot();

                                CharacterManagerRef.ChangeBagSize(CharacterManagerRef.CalculateBagSize()); // change the bag size

                                await Task.Delay(CharacterManagerRef.DepositDuration);

                                //change the UI pop up on top of the building
                                BuildingStockageUI buildingStockageUI = constructionInventory.gameObject.GetComponent<BuildingStockageUI>();
                                if (buildingStockageUI != null)
                                {
                                    buildingStockageUI.UpdateBuildingStatus();
                                }

                                if (constructionInventory.InventorySystem.AmountOfSlotsAvaliable() == 0)
                                {
                                    CharacterManagerRef.isAssignedToABuilding = false;
                                    CharacterManagerRef.buildingAssigned = null;
                                    return;
                                }
                            }

                        }
                        constructionInventory = null;
                        locationReached = true;

                        if (CharacterManagerRef.Inventory.InventorySystem.KnowIfInventoryIsEmpty())
                        {
                            CharacterManagerRef.HideBag();
                        }

                    }
                    await Task.Delay(250);
                }

            }
        }

        // STEP THREE : Check for resources in other buildings

        Transform[] buildingTransform = findingScript.GetTransformArray(Global.BUILDING_LAYER_MASK);
        List<Transform> buildingTransformsWithItems = new List<Transform>();

        foreach (Transform t in buildingTransform) 
        {
            ConstructionInventory inventoryFound = t.GetComponent<ConstructionInventory>();
            foreach(InventoryResourceType validtype in inventoryFound.validType)
            {
                if(constructionInventory.validType.Contains(validtype) && !inventoryFound.InventorySystem.KnowIfInventoryIsEmpty())
                {
                    foreach(InventorySlot slot in inventoryFound.InventorySystem.InventorySlots) 
                    { 
                    if(slot.ItemData.resourceType == validtype) 
                        {
                        if(!buildingTransformsWithItems.Contains(t)) 
                            {
                                buildingTransformsWithItems.Add(t);
                                break;
                            }
                        }
                    }
                }
            }
        }

        // Go to the closest buildings with the correspondaing items

        Transform[] FoundBuildings = buildingTransformsWithItems.ToArray();

        targetTransform = findingScript.GetClosestBuilding(FoundBuildings);
        buildingTransformsWithItems.Remove(targetTransform);




    }

    public void DoCancelBuild()
    {

    }

}
