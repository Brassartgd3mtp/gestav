using System.Collections;
using System.Collections.Generic;
using System.Resources;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
public class CharacterManager : UnitManager
{


    private Character character;


    private ResourceSpot resourceSpot;

    private Item item;

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float stoppingDistance;
    [SerializeField] private float stoppingMultiplicator = 0.3f;

    [SerializeField] private int resourceToGather = 1;

    private InventoryHolder inventory;
    private InventorySlot inventorySlot;

    public bool GatheringMode;
    public bool BuildingMode;
    private bool isGathering;
    private float miningDuration = 3f;
    private float timer;
    protected override Unit Unit
    {
        get { return character; }
        set { character = value is Character ? (Character)value : null; }
    }

    private void Awake()
    {
        inventory = gameObject.GetComponent<InventoryHolder>();
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
                    }

                    timer = miningDuration;
                }
            }
            else GoStoreResources();
        }
    }

    public void MoveTo(Vector3 targetPosition)
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

    public void GoStoreResources() // The method that tell the worker to go store the resources he gathered in a building
    {
        isGathering = false;
        Debug.Log("va a la mine");
        GameObject _BuildingToGo = GameObject.Find("Mine(Clone)");
        if (_BuildingToGo != null)
        {
            Vector3 _targetLocation = new Vector3(_BuildingToGo.transform.position.x, _BuildingToGo.transform.position.y, _BuildingToGo.transform.position.z);
            MoveTo(_targetLocation);

            InventoryHolder _buildInv = _BuildingToGo.GetComponent<InventoryHolder>();
            for (int i = 0; i < inventory.InventorySystem.InventorySlots.Count; i++)
            {
                Debug.Log(inventory.InventorySystem.InventorySlots[i].ItemData);
                    _buildInv.InventorySystem.AddToInventory(inventory.InventorySystem.InventorySlots[i].ItemData, 1);
                    inventory.InventorySystem.InventorySlots[i].ClearSlot();

            }
            _BuildingToGo = null;
            _buildInv = null;

        }
        else GatheringMode = false;
    }
}

