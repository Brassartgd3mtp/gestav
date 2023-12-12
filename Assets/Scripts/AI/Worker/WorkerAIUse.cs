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
    private CharacterUI characterUI;
    protected override void Awake()
    {
        base.Awake();
        findingScript = GetComponentInParent<Find>();
        WorkerManagerRef.canBeMovedByPlayer = false;
        characterUI = FindAnyObjectByType<CharacterUI>();
    }
    public override void ApplyBehaviour()
    {
        WorkerManagerRef.canBeMovedByPlayer = false;
        if(WorkerManagerRef.CanDoAction) 
        {
            if (WorkerManagerRef.isAssignedToABuilding && WorkerManagerRef.buildingAssigned != null && WorkerManagerRef.buildingAssigned.hasBeenBuilt)
            {
                UseBuilding();
                WorkerManagerRef.CanDoAction = false;
            }
            else if (WorkerManagerRef.isAssignedToABuilding && WorkerManagerRef.buildingAssigned != null && !WorkerManagerRef.buildingAssigned.hasBeenBuilt)
            {
                DoBuild();
                WorkerManagerRef.CanDoAction = false;
            }
            else if (WorkerManagerRef.isAssignedToABuilding && WorkerManagerRef.resourceAssigned != null)
            {
                Collect();
                WorkerManagerRef.CanDoAction = false;
            }
        }

    }
    public override BehaviourName CheckTransition()
    {

        if (!WorkerManagerRef.isAssignedToABuilding)
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
            switch (WorkerManagerRef.buildingAssigned.TypeOfBuilding)
            {
                case BuildingType.Collect:
                    if (WorkerManagerRef.GatheringMode)
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
        Transform targetTransform = WorkerManagerRef.buildingAssigned.gameObject.transform;
        float distanceToStop = targetTransform.GetComponent<BoxCollider>().size.z + 1.5f;

        WorkerManagerRef.MoveTo(targetTransform.position, distanceToStop);
        if (Vector3.Distance(transform.position, targetTransform.position) < distanceToStop)
        {
            buildingReached = true;
            WorkerManagerRef.EnterGatheringMode();
        }

    }

    public async void Collect()
    {
        currentActionText.text = "Collecting Items . . .";
        currentActionText.outlineColor = Color.black;
        currentActionText.color = Color.white;
        currentActionText.outlineWidth = 0.35f;

        Transform targetTransform = WorkerManagerRef.resourceAssigned.gameObject.transform;

        if (targetTransform != null) //Si a trouvé une ressource
        {
            float distanceToStop = targetTransform.GetComponentInParent<BoxCollider>().size.z + 1.5f;
            Vector3 targetLocation = targetTransform.position;
            WorkerManagerRef.MoveTo(targetLocation, distanceToStop); //Va a la position de la mine
            WorkerManagerRef.EnterGatheringMode();
        }

        while (WorkerManagerRef.resourceAssigned != null)
        {
            if (WorkerManagerRef.Inventory.InventorySystem.AmountOfSlotsAvaliable() == 0)
            {
                AssignWorkerInventory assignInv = WorkerManagerRef.resourceAssigned.GetComponent<AssignWorkerInventory>();

                for (int i = 0; i < assignInv.InventorySystem.InventorySize; i++)
                {
                    if (assignInv.InventorySystem.InventorySlots[i].ItemData != null)
                    {
                        assignInv.InventorySystem.InventorySlots[i].ClearSlot();
                        break;
                    }
                }

                WorkerManagerRef.resourceAssigned = null;
                WorkerManagerRef.ExitGatheringMode();
                WorkerManagerRef.isAssignedToABuilding = false;
            }

            await Task.Delay(100);
        }

        WorkerManagerRef.resourceAssigned = null;
        WorkerManagerRef.ExitGatheringMode();
        WorkerManagerRef.isAssignedToABuilding = false;

        WorkerManagerRef.CanDoAction = true;

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
        BuildingInventory buildinv = WorkerManagerRef.buildingAssigned.GetComponent<BuildingInventory>();


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
            WorkerManagerRef.MoveTo(targetLocation, distanceToStop); //Va a la position de la mine
        }
        WorkerManagerRef.CanDoAction = true;
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
        WorkerManagerRef.ExitGatheringMode();

        Transform targetTransform = WorkerManagerRef.buildingAssigned.gameObject.transform;

        if (targetTransform != null) //Si a trouvé le batiment
        {
            float distanceToStop = targetTransform.GetComponent<BoxCollider>().size.z + 1.5f;
            bool locationReached = false;
            Vector3 targetLocation = targetTransform.position;
            WorkerManagerRef.MoveTo(targetLocation, distanceToStop); //Va a la position de la mine

            while (locationReached == false)
            {
                if (Vector3.Distance(transform.position, targetLocation) <= distanceToStop) //Si l'ouvrier est suffisament près de la mine
                {
                    BuildingInventory _buildInv = targetTransform.GetComponent<BuildingInventory>();
                    for (int i = 0; i < WorkerManagerRef.Inventory.InventorySystem.InventorySlots.Count; i++) //transfert les objets de son inventaire à celui de la mine
                    {

                        //Put worker items into the building

                        if (WorkerManagerRef.Inventory.InventorySystem.InventorySlots[i].ItemData != null && _buildInv.validType.Contains(WorkerManagerRef.Inventory.InventorySystem.InventorySlots[i].ItemData.resourceType))
                        {
                            _buildInv.InventorySystem.AddToInventory(WorkerManagerRef.Inventory.InventorySystem.InventorySlots[i].ItemData, 1);
                            WorkerManagerRef.Inventory.InventorySystem.InventorySlots[i].ClearSlot();

                            WorkerManagerRef.ChangeBagSize(WorkerManagerRef.CalculateBagSize()); // change the bag size

                            await Task.Delay(WorkerManagerRef.DepositDuration);

                            //change the UI pop up on top of the building
                            BuildingStockageUI buildingStockageUI = _buildInv.gameObject.GetComponent<BuildingStockageUI>();
                            if (buildingStockageUI != null)
                            {
                                buildingStockageUI.UpdateSpaceInUI();
                            }

                            //Dynamic display of character inventory if he is selected
                            WorkerManagerRef.DisplayThisIventory();

                            //Dynamic display of building inventory if it is selected
                            if (Global.SELECTED_BUILDINGS.Count == 1 && Global.SELECTED_BUILDINGS[0] == _buildInv.gameObject.GetComponent<UnitManager>())
                            {
                                WorkerManagerRef.ShowInventoryUI(_buildInv.InventorySystem);
                            }

                        }

                    }
                    _buildInv = null;
                    locationReached = true;

                    // If inventory is empty go mine, else find the new destination to empty his inventory
                    if (WorkerManagerRef.Inventory.InventorySystem.KnowIfInventoryIsEmpty())
                    {
                        WorkerManagerRef.HideBag();
                        WorkerManagerRef.EnterGatheringMode();
                    }
                    else
                    {

                    }



                }
                await Task.Delay(250);
            }

        }
        WorkerManagerRef.CanDoAction = true;
    }

    public async void DoBuild()
    {
        currentActionText.text = "Building . . .";
        currentActionText.outlineColor = Color.black;
        currentActionText.color = Color.white;
        currentActionText.outlineWidth = 0.35f;

        Transform targetTransform = WorkerManagerRef.buildingAssigned.transform;
        ConstructionInventory constructionInventory = WorkerManagerRef.buildingAssigned.GetComponent<ConstructionInventory>();

        foreach(ItemTypeAndCount itemToTransfer in WorkerManagerRef.buildingAssigned.Building.Data.resourcesToBuild)
        {

            int amountOfItemsAlreadyInInventory = 0;

            foreach(InventorySlot slot in constructionInventory.InventorySystem.InventorySlots)
            {
                if(slot.ItemData == itemToTransfer.item)
                {
                    amountOfItemsAlreadyInInventory++;
                }
            }


            int amountToTransfer = itemToTransfer.count - amountOfItemsAlreadyInInventory;
            InventoryItemData itemdata = itemToTransfer.item;

            bool hasResourcesToBuild = false;

            for (int amountTransfered = 0; amountTransfered < amountToTransfer; amountTransfered += 0)
            {
                Debug.Log(amountTransfered);
                Debug.Log(amountToTransfer);
                for (int i = 0; i < WorkerManagerRef.Inventory.InventorySystem.InventorySlots.Count; i++)
                {
                    if (constructionInventory != null && WorkerManagerRef.Inventory.InventorySystem.InventorySlots[i].ItemData !=null && WorkerManagerRef.Inventory.InventorySystem.InventorySlots[i].ItemData.resourceType == itemdata.resourceType)
                    {
                        hasResourcesToBuild = true;
                        break;
                    }
                }

                if (hasResourcesToBuild)
                {
                    if (targetTransform != null) //Si a trouvé le batiment
                    {
                        float distanceToStop = targetTransform.GetComponent<BoxCollider>().size.z + 1.5f;
                        bool locationReached = false;
                        Vector3 targetLocation = targetTransform.position;
                        WorkerManagerRef.MoveTo(targetLocation, distanceToStop); //Va a la position du building

                        while (!locationReached)
                        {
                            if (Vector3.Distance(transform.position, targetLocation) <= distanceToStop) //Si l'ouvrier est suffisament près de la mine
                            {
                                for (int i = 0; i < WorkerManagerRef.Inventory.InventorySystem.InventorySlots.Count; i++) //transfert les objets de son inventaire à celui de la mine
                                {

                                    //Put worker items into the building

                                    if (WorkerManagerRef.Inventory.InventorySystem.InventorySlots[i].ItemData != null && constructionInventory.validType.Contains(WorkerManagerRef.Inventory.InventorySystem.InventorySlots[i].ItemData.resourceType))
                                    {
                                        constructionInventory.InventorySystem.AddToInventory(WorkerManagerRef.Inventory.InventorySystem.InventorySlots[i].ItemData, 1);
                                        WorkerManagerRef.Inventory.InventorySystem.InventorySlots[i].ClearSlot();

                                        WorkerManagerRef.ChangeBagSize(WorkerManagerRef.CalculateBagSize()); // change the bag size

                                        await Task.Delay(WorkerManagerRef.DepositDuration);

                                        amountToTransfer++;
                                        constructionInventory.GetComponent<BuildingStockageUI>().UpdateSpaceInUI();
                                        //change the UI pop up on top of the building


                                    }

                                }
                                locationReached = true;

                                if (WorkerManagerRef.Inventory.InventorySystem.KnowIfInventoryIsEmpty())
                                {
                                    WorkerManagerRef.HideBag();
                                }

                            }
                            await Task.Delay(250);
                        }

                    }
                    if (constructionInventory.InventorySystem.AmountOfSlotsAvaliable() == 0 || amountTransfered >= amountToTransfer)
                    {
                        WorkerManagerRef.buildingAssigned.hasBeenBuilt = true;
                        WorkerManagerRef.isAssignedToABuilding = false;


                        if (WorkerManagerRef.buildingAssigned != null)
                        {
                            AssignWorkerInventory assignInv = WorkerManagerRef.buildingAssigned.GetComponent<AssignWorkerInventory>();

                            for (int i = 0; i < assignInv.InventorySystem.InventorySize; i++)
                            {
                                if (assignInv.InventorySystem.InventorySlots[i].ItemData != null)
                                {
                                    assignInv.InventorySystem.InventorySlots[i].ClearSlot();
                                    break;
                                }
                            }

                            WorkerManagerRef.buildingAssigned = null;
                        }
                        if (WorkerManagerRef.resourceAssigned != null)
                        {
                            AssignWorkerInventory assignInv = WorkerManagerRef.resourceAssigned.GetComponent<AssignWorkerInventory>();

                            for (int i = 0; i < assignInv.InventorySystem.InventorySize; i++)
                            {
                                if (assignInv.InventorySystem.InventorySlots[i].ItemData != null)
                                {
                                    assignInv.InventorySystem.InventorySlots[i].ClearSlot();
                                    break;
                                }
                            }
                            WorkerManagerRef.resourceAssigned = null;
                            WorkerManagerRef.CanDoAction = true;
                        }

                        Destroy(constructionInventory);
                        constructionInventory = null;
                    }
                }

                // STEP THREE : Check for resources in other buildings

                Transform[] buildingTransform = findingScript.GetTransformArray(Global.BUILDING_LAYER_MASK);
                List<Transform> buildingTransformsWithItems = new List<Transform>();
                buildingTransformsWithItems.Remove(WorkerManagerRef.buildingAssigned.transform);

                foreach (Transform t in buildingTransform)
                {
                    BuildingInventory inventoryFound = t.GetComponent<BuildingInventory>();

                    if(inventoryFound != null && constructionInventory != null)
                    {
                        foreach (InventoryResourceType validtype in inventoryFound.validType)
                        {
                            if (constructionInventory.validType.Contains(validtype) && !inventoryFound.InventorySystem.KnowIfInventoryIsEmpty())
                            {
                                foreach (InventorySlot slot in inventoryFound.InventorySystem.InventorySlots)
                                {
                                    if (slot.ItemData.resourceType == validtype)
                                    {
                                        if (!buildingTransformsWithItems.Contains(t))
                                        {
                                            buildingTransformsWithItems.Add(t);
                                            break;
                                        }
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

                if (targetTransform != null)
                {
                    float distanceToStop = targetTransform.GetComponent<BoxCollider>().size.z + 1.5f;
                    bool locationReached = false;
                    Vector3 targetLocation = targetTransform.position;
                    WorkerManagerRef.MoveTo(targetLocation, distanceToStop); //Va a la position du building


                    while (!locationReached)
                    {

                        BuildingInventory targetInventory = targetTransform.GetComponent<BuildingInventory>();

                        if (Vector3.Distance(transform.position, targetLocation) <= distanceToStop && WorkerManagerRef.Inventory.InventorySystem.AmountOfSlotsAvaliable() >= 0) //Si l'ouvrier est suffisament près de la mine
                        {
                            for (int i = 0; i < WorkerManagerRef.Inventory.InventorySystem.InventorySlots.Count; i++) //transfert les objets de son inventaire à celui de la mine
                            {

                                //Take items from the building

                                if (WorkerManagerRef.Inventory.InventorySystem.InventorySlots[i].ItemData == null)
                                {
                                    int lastSlot = targetInventory.InventorySystem.InventorySize - (targetInventory.InventorySystem.AmountOfSlotsAvaliable() + 1);
                                    if (lastSlot < 0) lastSlot = 0;

                                    if (targetInventory.InventorySystem.InventorySlots[lastSlot].ItemData != null)
                                    {
                                        WorkerManagerRef.Inventory.InventorySystem.AddToInventory(targetInventory.InventorySystem.InventorySlots[lastSlot].ItemData, 1);
                                        targetInventory.InventorySystem.InventorySlots[lastSlot].ClearSlot();
                                        BuildingStockageUI buildingStockageUI = targetTransform.GetComponent<BuildingStockageUI>();
                                        buildingStockageUI.UpdateSpaceInUI();
                                    }
                                }
                                locationReached = true;

                                if (!WorkerManagerRef.Inventory.InventorySystem.KnowIfInventoryIsEmpty())
                                {
                                    WorkerManagerRef.ShowBag();
                                }
                            }

                        }
                        await Task.Delay(250);
                    }
                }
                // Desposit to building

                targetTransform = WorkerManagerRef.buildingAssigned.GetComponent<Transform>();
                constructionInventory = targetTransform.GetComponent<ConstructionInventory>();
                if (targetTransform != null && constructionInventory != null)
                {
                    float distanceToStop = targetTransform.GetComponent<BoxCollider>().size.z + 1.5f;
                    bool locationReached = false;
                    Vector3 targetLocation = targetTransform.position;
                    WorkerManagerRef.MoveTo(targetLocation, distanceToStop); //Va a la position du building

                    while (!locationReached)
                    {
                        if (Vector3.Distance(transform.position, targetLocation) <= distanceToStop) //Si l'ouvrier est suffisament près de la mine
                        {
                            for (int i = 0; i < WorkerManagerRef.Inventory.InventorySystem.InventorySlots.Count; i++) //transfert les objets de son inventaire à celui de la mine
                            {

                                //Put worker items into the building

                                if (WorkerManagerRef.Inventory.InventorySystem.InventorySlots[i].ItemData != null && constructionInventory.validType.Contains(WorkerManagerRef.Inventory.InventorySystem.InventorySlots[i].ItemData.resourceType))
                                {
                                    constructionInventory.InventorySystem.AddToInventory(WorkerManagerRef.Inventory.InventorySystem.InventorySlots[i].ItemData, 1);
                                    WorkerManagerRef.Inventory.InventorySystem.InventorySlots[i].ClearSlot();

                                    WorkerManagerRef.ChangeBagSize(WorkerManagerRef.CalculateBagSize()); // change the bag size
                                    

                                    await Task.Delay(WorkerManagerRef.DepositDuration);
                                    amountTransfered++;
                                    constructionInventory.GetComponent<BuildingStockageUI>().UpdateSpaceInUI();
                                    //change the UI pop up on top of the building


                                }

                            }
                            locationReached = true;

                            if (WorkerManagerRef.Inventory.InventorySystem.KnowIfInventoryIsEmpty())
                            {
                                WorkerManagerRef.HideBag();
                            }

                        }
                        await Task.Delay(250);
                    }
                    if (constructionInventory.InventorySystem.AmountOfSlotsAvaliable() == 0 || amountTransfered >= amountToTransfer)
                    {
                        WorkerManagerRef.isAssignedToABuilding = false;
                        WorkerManagerRef.buildingAssigned = null;
                        WorkerManagerRef.CanDoAction = true;

                        Destroy(constructionInventory);
                        constructionInventory = null;
                    }


                }

                if(amountTransfered >= amountToTransfer)
                {
                    WorkerManagerRef.buildingAssigned.hasBeenBuilt = true;
                    WorkerManagerRef.isAssignedToABuilding = false;
                    if (WorkerManagerRef.buildingAssigned != null)
                    {
                        AssignWorkerInventory assignInv = WorkerManagerRef.buildingAssigned.GetComponent<AssignWorkerInventory>();

                        for (int i = 0; i < assignInv.InventorySystem.InventorySize; i++)
                        {
                            if (assignInv.InventorySystem.InventorySlots[i].ItemData != null)
                            {
                                assignInv.InventorySystem.InventorySlots[i].ClearSlot();
                                break;
                            }
                        }

                        WorkerManagerRef.buildingAssigned = null;
                    }
                    if (WorkerManagerRef.resourceAssigned != null)
                    {
                        AssignWorkerInventory assignInv = WorkerManagerRef.resourceAssigned.GetComponent<AssignWorkerInventory>();

                        for (int i = 0; i < assignInv.InventorySystem.InventorySize; i++)
                        {
                            if (assignInv.InventorySystem.InventorySlots[i].ItemData != null)
                            {
                                assignInv.InventorySystem.InventorySlots[i].ClearSlot();
                                break;
                            }
                        }

                        WorkerManagerRef.resourceAssigned = null;
                    }
                }


            }
            if (constructionInventory.InventorySystem.AmountOfSlotsAvaliable() == 0 )
            {
                WorkerManagerRef.buildingAssigned.hasBeenBuilt = true;
                WorkerManagerRef.isAssignedToABuilding = false;
                if (WorkerManagerRef.buildingAssigned != null)
                {
                    AssignWorkerInventory assignInv = WorkerManagerRef.buildingAssigned.GetComponent<AssignWorkerInventory>();

                    for (int i = 0; i < assignInv.InventorySystem.InventorySize; i++)
                    {
                        if (assignInv.InventorySystem.InventorySlots[i].ItemData != null)
                        {
                            assignInv.InventorySystem.InventorySlots[i].ClearSlot();
                            break;
                        }
                    }

                    WorkerManagerRef.buildingAssigned = null;
                }
                if (WorkerManagerRef.resourceAssigned != null)
                {
                    AssignWorkerInventory assignInv = WorkerManagerRef.resourceAssigned.GetComponent<AssignWorkerInventory>();

                    for (int i = 0; i < assignInv.InventorySystem.InventorySize; i++)
                    {
                        if (assignInv.InventorySystem.InventorySlots[i].ItemData != null)
                        {
                            assignInv.InventorySystem.InventorySlots[i].ClearSlot();
                            break;
                        }
                    }

                    WorkerManagerRef.resourceAssigned = null;
                }

                WorkerManagerRef.CanDoAction = true;
                Destroy(constructionInventory);
                constructionInventory = null;
            }

        }


        WorkerManagerRef.buildingAssigned.hasBeenBuilt = true;
        WorkerManagerRef.isAssignedToABuilding = false;
        if (WorkerManagerRef.buildingAssigned != null)
        {
            AssignWorkerInventory assignInv = WorkerManagerRef.buildingAssigned.GetComponent<AssignWorkerInventory>();

            for (int i = 0; i < assignInv.InventorySystem.InventorySize; i++)
            {
                if (assignInv.InventorySystem.InventorySlots[i].ItemData != null)
                {
                    assignInv.InventorySystem.InventorySlots[i].ClearSlot();
                    break;
                }
            }

            WorkerManagerRef.buildingAssigned = null;
        }
        if (WorkerManagerRef.resourceAssigned != null)
        {
            AssignWorkerInventory assignInv = WorkerManagerRef.resourceAssigned.GetComponent<AssignWorkerInventory>();

            for (int i = 0; i < assignInv.InventorySystem.InventorySize; i++)
            {
                if (assignInv.InventorySystem.InventorySlots[i].ItemData != null)
                {
                    assignInv.InventorySystem.InventorySlots[i].ClearSlot();
                    break;
                }
            }

            WorkerManagerRef.resourceAssigned = null;
        }

        WorkerManagerRef.CanDoAction = true;
        Destroy(constructionInventory);
        constructionInventory = null;
    }
    public void DoCancelBuild()
    {

    }
}
