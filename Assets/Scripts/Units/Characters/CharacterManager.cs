using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class CharacterManager : UnitManager
{


    private Character character;
    [SerializeField] private Find findingScript;

    private ResourceSpot resourceSpot;
    private Item item;

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float stoppingDistance;
    [SerializeField] private float stoppingMultiplicator = 0.3f;

    [SerializeField] private int resourceToGather = 1;

    private BuildingInventory inventory;
    private DynamicInventoryDisplay inventoryDisplay;

    public bool GatheringMode;
    public bool BuildingMode;
    private bool isGathering;
    private InventoryResourceType lastGatheredResource;

    private float miningDuration = 3f;
    private int depositDuration = 350; //en milisecondes

    private float timer;

    [SerializeField] GameObject bagContainer;
    private Vector3 startingBagScale;
    [SerializeField] Animator animator;


    protected override Unit Unit
    {
        get { return character; }
        set { character = value is Character ? (Character)value : null; }
    }

    private void Awake()
    {
        inventory = gameObject.GetComponent<BuildingInventory>();
        resourceToGather = 1;
        startingBagScale = bagContainer.transform.localScale;
    }

    private void Update()
    {
        if(isGathering)
        {
            if (inventory.InventorySystem.HasFreeSlot(out InventorySlot _freeSlot))
            {

                animator.SetBool("isMining", true);

                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    if (resourceSpot.Quantity > 0)
                    {
                        resourceSpot.GatherResources(resourceToGather);
                        inventory.InventorySystem.AddToInventory(item.item, resourceToGather);
                        lastGatheredResource = item.item.resourceType;
                       
                        //change the bag according to how much there is inside
                        ShowBag();
                        ChangeBagSize(CalculateBagSize());

                        //Dynamic display of character inventory if he is selected
                        if ( Global.SELECTED_UNITS.Count == 1 && Global.SELECTED_UNITS[0] == gameObject.GetComponent<UnitManager>())
                        {
                            ShowInventoryUI(inventory.InventorySystem);
                        }
                    }

                    timer = miningDuration;
                }
            }
            else GoStoreResources(); // a bouger dans un endroit ou l'on check si l'inventaire est plein
        }
    }

    public async void MoveTo(Vector3 targetPosition, float _rangeToStop)
    {
        stoppingDistance = stoppingMultiplicator * (Global.SELECTED_UNITS.Count -1f);
        // Stop the current movement
        agent.isStopped = true;
        if(Global.SELECTED_UNITS.Count > 1)
        {

            Vector3 randomOffset = new Vector3( Random.Range(-stoppingDistance, stoppingDistance),0,Random.Range(-stoppingDistance, stoppingDistance));

            agent.destination = targetPosition + randomOffset;
        }
        else
        // Set the new destination
        agent.destination = targetPosition;

        // Resume movement
        agent.isStopped = false;
        while(agent.velocity != Vector3.zero)
        {
            if(Vector3.Distance(transform.position, targetPosition) < _rangeToStop)
            {
                agent.isStopped = true;
                return;
            }
            await Task.Delay(100);
        }
        
    }



    private void OnTriggerEnter(Collider other)
    {
        EnterGatheringMode();
        Debug.Log(GatheringMode);
        if (other.tag == "ResourceArea" && GatheringMode == true && inventory.InventorySystem.HasFreeSlot(out InventorySlot _freeSlot))
        {
            Debug.Log("collision");
            resourceSpot = other.gameObject.GetComponentInChildren<ResourceSpot>();
            item = other.gameObject.GetComponentInChildren<Item>();
            StartGathering();
        }
        else Debug.Log("Une condition n'est pas remplie???");
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "ResourceArea")
        StopGathering();
    }

    public void StartGathering() //make the unit start gathering resources
    {
        isGathering = true;
        timer = miningDuration;
    }

    public void StopGathering() //make the unit stop gathering resources
    {
        animator.SetBool("isMining", false);
        isGathering = false;
        timer = miningDuration;
    }
    public void EnterGatheringMode() // Enter the Gathering resources mode
    {
        GatheringMode = true;
    }
    public void ExitGatheringMode() // QUit the Gathering resources mode
    {
        GatheringMode = false;
        StopGathering();
    }

    public async void GoStoreResources() // The method that tell the worker to go store the resources he gathered in a building
    {

        ExitGatheringMode();

        Transform targetTransform = DecideWhichBuildingToGo();
        if (targetTransform != null) //Si a trouv� le batiment
        {
                float distanceToStop = targetTransform.GetComponent<BoxCollider>().size.z + 1.5f;
                bool locationReached = false;
                Vector3 targetLocation = targetTransform.position;
                MoveTo(targetLocation, distanceToStop); //Va a la position de la mine

            while (locationReached == false)
            {
                if (Vector3.Distance(transform.position, targetLocation) <= distanceToStop) //Si l'ouvrier est suffisament pr�s de la mine
                {
                    Debug.Log("DESTINATION REACHED");
                    InventoryHolder _buildInv = targetTransform.GetComponent<InventoryHolder>();
                    for (int i = 0; i < inventory.InventorySystem.InventorySlots.Count; i++) //transfert les objets de son inventaire � celui de la mine
                    {

                        //Put worker items into the building
                        Debug.Log(inventory.InventorySystem.InventorySlots[i].ItemData);

                        if(inventory.InventorySystem.InventorySlots[i].ItemData != null && inventory.InventorySystem.InventorySlots[i].ItemData.resourceType == _buildInv.validType)
                        {
                            _buildInv.InventorySystem.AddToInventory(inventory.InventorySystem.InventorySlots[i].ItemData, 1);
                            inventory.InventorySystem.InventorySlots[i].ClearSlot();

                            ChangeBagSize(CalculateBagSize()); // change the bag size

                            await Task.Delay(depositDuration);

                            //change the UI pop up on top of the building
                            BuildingStockageUI buildingStockageUI = _buildInv.gameObject.GetComponent<BuildingStockageUI>();
                            if (buildingStockageUI != null)
                            {
                                buildingStockageUI.UpdateSpaceInUI();
                            }

                            //Dynamic display of character inventory if he is selected
                            if (Global.SELECTED_UNITS.Count == 1 && Global.SELECTED_UNITS[0] == gameObject.GetComponent<UnitManager>())
                            {
                                ShowInventoryUI(inventory.InventorySystem);
                            }

                            //Dynamic display of building inventory if it is selected
                            if (Global.SELECTED_UNITS.Count == 1 && Global.SELECTED_UNITS[0] == _buildInv.gameObject.GetComponent<UnitManager>())
                            {
                                ShowInventoryUI(_buildInv.InventorySystem);
                            }

                        }

                    }
                    _buildInv = null;
                    locationReached = true;

                    // If inventory is empty go mine, else find the new destination to empty his inventory
                    if (inventory.InventorySystem.KnowIfInventoryIsEmpty())
                    {
                        Debug.Log("Inventory empty");
                        HideBag();
                        GoMining();
                    }
                    else
                    Debug.Log("Inventory not empty");
                    GoStoreResources();

                }
                await Task.Delay(250);
            }

            targetTransform = null;
        }
         else ExitGatheringMode();
    }


    public void GoMining()
    {
        EnterGatheringMode();
       Transform[] resourcesTranform = findingScript.GetTransformArray(Global.RESOURCE_LAYER_MASK);
        Item[] resourcesTypes = new Item[resourcesTranform.Length];
        List<Item> correspondingItems = new List<Item>();
        for (int i = 0; i < resourcesTranform.Length; i++)
        {
            resourcesTypes[i] = resourcesTranform[i].gameObject.GetComponentInChildren<Item>();
        }
        foreach (Item item in resourcesTypes)
        {
            if(item != null && item.item.resourceType == lastGatheredResource) 
            {
            correspondingItems.Add(item);
            }
        }


        Transform targetTransform = GetClosestResource(correspondingItems); // Trouve la mine la plus proche
        if (targetTransform != null) //Si a trouv� une ressource
        {
            float distanceToStop = targetTransform.GetComponentInParent<BoxCollider>().size.z + 1.5f;
            Vector3 targetLocation = targetTransform.position;
            MoveTo(targetLocation, distanceToStop); //Va a la position de la mine
        }
    }

    public Transform DecideWhichBuildingToGo()
    {

        float detectionRadius = 10000f;

        //detect building colliders, find the corresponding transforms and inventories
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, Global.BUILDING_LAYER_MASK);
        Transform[] buildingTransform = new Transform[colliders.Length];
        List<InventoryHolder> correspondingInventories = new List<InventoryHolder>();
        InventoryHolder[] inventories = new InventoryHolder[buildingTransform.Length];

        for (int i = 0; i < colliders.Length; i++)
        {
            buildingTransform[i] = colliders[i].transform;
        }
            if (buildingTransform.Length > 0)
            {

            //get the inventories 

            for (int i = 0; i < buildingTransform.Length; i++)
            {
                inventories[i] = buildingTransform[i].gameObject.GetComponent<InventoryHolder>();
            }
                //add to a list the inventories that accepts the items currently in the worker's invntory
                foreach (InventoryHolder inv in inventories)
                {
                    if (inventories.Length > 0 && GetItemInFirstOccupiedSlot() != null && GetItemInFirstOccupiedSlot().resourceType == inv.validType)
                    {
                        correspondingInventories.Add(inv);
                    }
                }


            }
            //return the transform of the closest building that can accept the item in the inventory

            Transform target = GetClosestBuilding(correspondingInventories);
            return target;

    }


public InventoryItemData GetItemInFirstOccupiedSlot()
    {
        foreach (InventorySlot slot in inventory.InventorySystem.InventorySlots)
        {
            if (slot.ItemData != null)
            {
                Debug.Log(slot.ItemData);
                return slot.ItemData;
            }
        }
        return null;
    }

public Transform GetClosestBuilding(List<InventoryHolder> correspondingInventories)
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (InventoryHolder inv in correspondingInventories)
        {
            float dist = Vector3.Distance(inv.gameObject.transform.position, currentPos);
            if (dist < minDist)
            {
                tMin = inv.gameObject.transform;
                minDist = dist;
            }
        }
        return tMin;
    }

    public Transform GetClosestResource(List<Item> correspondingItems)
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (Item item in correspondingItems)
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

    public void ShowInventoryUI(InventorySystem InvToDisplay)
    {
        InventoryHolder.OnDynamicInventoryDisplayRequested?.Invoke(InvToDisplay);
    }

    public void ShowBag()
    {
        bagContainer.SetActive(true);
    }

    public void HideBag()
    {
        bagContainer?.SetActive(false);
    }

    public int CalculateBagSize()
    {
        int _itemsInBag = 0;
        int _totalItemsInBag = 0;
        int _bagSize = 0;
        InventorySlot[] inv = inventory.InventorySystem.InventorySlots.ToArray();
        for (int i = 0; i < inv.Length; i++)
        {
            _totalItemsInBag++;
            if (inv[i].ItemData != null) _itemsInBag++;
        }

       float ratio =  _itemsInBag / _totalItemsInBag * 100;

        if (ratio < 34) _bagSize = 1;
        else if (ratio < 67) _bagSize = 2;
        else _bagSize = 3;
        Debug.Log(ratio);
        return _bagSize;
    }

    public void ChangeBagSize(int _bagSize)
    {
        switch(_bagSize) 
        {
        case 1:
                bagContainer.transform.localScale = startingBagScale; break;
        case 2:
                bagContainer.transform.localScale = new Vector3(startingBagScale.x *1.5f, startingBagScale.y * 1.5f, startingBagScale.z * 1.5f); break;
        case 3:
                bagContainer.transform.localScale = new Vector3(startingBagScale.x * 2f, startingBagScale.y * 2f, startingBagScale.z * 2f); break;
        default:
                bagContainer.transform.localScale = startingBagScale; break;
        }
    }

}

