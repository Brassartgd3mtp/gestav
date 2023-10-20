using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using System.Threading.Tasks;
using Unity.VisualScripting;
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

    private InventoryHolder inventory;
    private InventorySlot inventorySlot;

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
        inventory = gameObject.GetComponent<InventoryHolder>();
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

        Transform targetTransform = findingScript.GetClosestBuilding(findingScript.GetTransformArray(Global.MINE_LAYER_MASK)); // Trouve la mine la plus proche
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
                        Debug.Log(inventory.InventorySystem.InventorySlots[i].ItemData);
                        _buildInv.InventorySystem.AddToInventory(inventory.InventorySystem.InventorySlots[i].ItemData, 1);
                        inventory.InventorySystem.InventorySlots[i].ClearSlot();
                        await Task.Delay(depositDuration);
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
}

