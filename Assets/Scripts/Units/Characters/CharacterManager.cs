using System.Collections;
using System.Collections.Generic;
using System.Resources;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
public class CharacterManager : UnitManager
{


    private Character character;

    public WorkerInventory workerInventory;
    public InventoryObject inventory;
    private Item item;


    private ResourceSpot resourceSpot;


    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float stoppingDistance;
    [SerializeField] private float stoppingMultiplicator = 0.3f;

    [SerializeField] private int resourceToGather = 1;

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

    private void Update()
    {
        if(isGathering)
        {
            timer -= Time.deltaTime;
            if (timer <=0f)
            {
                resourceSpot.GatherResources(resourceToGather);
                timer = miningDuration;
                if(item != null)
                {
                    inventory.AddItem(item.item, resourceToGather);
                }
            }
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
        if (other.tag == "ResourceArea" && GatheringMode == true)
        {
            Debug.Log("collision");
            resourceSpot = other.gameObject.GetComponentInChildren<ResourceSpot>();
            var _item = other.gameObject.GetComponentInChildren<Item>();
            item = _item;
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
        Debug.Log("Gathering mode ativé");
    }
    public void ExitGatheringMode() // QUit the Gathering resources mode
    {
        GatheringMode = false;
        Debug.Log("Gathering mode désactivé");
        StopGathering();
    }


}

