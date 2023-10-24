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

    private float miningDuration = 3f;
    private int depositDuration = 250; //en milisecondes

    private float timer;
    protected override Unit Unit
    {
        get { return character; }
        set { character = value is Character ? (Character)value : null; }
    }

    private void Awake()
    {
        inventory = gameObject.GetComponent<BuildingInventory>();
        resourceToGather = 1;
    }

    private void Update()
    {
        if(isGathering)
        {
            if (inventory.InventorySystem.HasFreeSlot(out InventorySlot _freeSlot))
            {

                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    if (resourceSpot.Quantity > 0)
                    {
                        resourceSpot.GatherResources(resourceToGather);
                        inventory.InventorySystem.AddToInventory(item.item, resourceToGather);

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
        if (other.tag == "ResourceArea" && GatheringMode == true && inventory.InventorySystem.HasFreeSlot(out InventorySlot _freeSlot))
        {
            Debug.Log("collision");
            resourceSpot = other.gameObject.GetComponentInChildren<ResourceSpot>();
            item = other.gameObject.GetComponentInChildren<Item>();
            StartGathering();
        }
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

        isGathering = false;
        Debug.Log("va a la mine");

        Transform targetTransform = DecideWhereToGo();
         // Transform targetTransform = findingScript.GetClosestBuilding(findingScript.GetTransformArray(Global.MINE_LAYER_MASK)); // Trouve la mine la plus proche
        if (targetTransform != null) //Si a trouvé une mine
        {
                float distanceToStop = targetTransform.GetComponent<BoxCollider>().size.z + 1.5f;
                bool locationReached = false;
                Vector3 targetLocation = targetTransform.position;
                MoveTo(targetLocation, distanceToStop); //Va a la position de la mine

            while (locationReached == false)
            {
                if (Vector3.Distance(transform.position, targetLocation) <= distanceToStop) //Si l'ouvrier est suffisament près de la mine
                {
                    Debug.Log("DESTINATION REACHED");
                    InventoryHolder _buildInv = targetTransform.GetComponent<InventoryHolder>();
                    for (int i = 0; i < inventory.InventorySystem.InventorySlots.Count; i++) //transfert les objets de son inventaire à celui de la mine
                    {

                        //Put worker items into the building
                        Debug.Log(inventory.InventorySystem.InventorySlots[i].ItemData);

                        if(inventory.InventorySystem.InventorySlots[i].ItemData.resourceType == _buildInv.validType)
                        {
                            _buildInv.InventorySystem.AddToInventory(inventory.InventorySystem.InventorySlots[i].ItemData, 1);
                            inventory.InventorySystem.InventorySlots[i].ClearSlot();
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
                    GoMining();
                }
                await Task.Delay(100);
            }

            targetTransform = null;
        }
      else GatheringMode = false;
    }


    public void GoMining()
    {
        Transform targetTransform = findingScript.GetClosestBuilding(findingScript.GetTransformArray(Global.RESOURCE_LAYER_MASK)); // Trouve la mine la plus proche
        if (targetTransform != null) //Si a trouvé une ressource
        {
            float distanceToStop = targetTransform.GetComponent<BoxCollider>().size.z + 1.5f;
            Vector3 targetLocation = targetTransform.position;
            MoveTo(targetLocation, distanceToStop); //Va a la position de la mine

        }
    }

    public Transform DecideWhereToGo()
    {

        float detectionRadius = 10000f;

        //detect building colliders, find the corresponding transforms and inventories
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, Global.BUILDING_LAYER_MASK);
        Debug.Log(colliders);
        Transform[] buildingTransform = new Transform[colliders.Length];
        List<InventoryHolder> correspondingInventories = new List<InventoryHolder>();

        for (int i = 0; i < colliders.Length; i++)
        {
            buildingTransform[i] = colliders[i].transform;

            if (buildingTransform != null)
            {

                //get the inventories 

                InventoryHolder[] inventories = new InventoryHolder[buildingTransform.Length];
                inventories[i] = buildingTransform[i].gameObject.GetComponent<InventoryHolder>();


                //add to a list the inventories that accepts the items currently in the worker's invntory

                foreach (InventoryHolder inv in inventories)
                {
                    if (inv != null && GetItemInFirstOccupiedSlot().resourceType == inv.validType)
                    {
                        correspondingInventories.Add(inv);
                    }
                }


            }
            //return the transform of the closest building that can accept the item in the inventory

            Transform target = GetClosestBuilding(correspondingInventories);
            return target;

        }
        return null;
    }


public InventoryItemData GetItemInFirstOccupiedSlot()
    {
        foreach (InventorySlot slot in inventory.InventorySystem.InventorySlots)
        {
            if (slot.ItemData != null)
            {
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

    public void ShowInventoryUI(InventorySystem InvToDisplay)
    {
        InventoryHolder.OnDynamicInventoryDisplayRequested?.Invoke(InvToDisplay);
    }
}

