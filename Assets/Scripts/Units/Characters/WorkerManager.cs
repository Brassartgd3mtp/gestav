using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.UI;

public class WorkerManager : CharacterManager
{
    [Header("IA")]

    private WorkerAIC workerAIC;
    public WorkerBehaviour workerBehaviour;
    public bool CanDoAction = true;

    [Header("Collect")]

    [SerializeField] private int resourceToGather = 1;
    public int ResourceToGather => resourceToGather;

    public bool GatheringMode;
    public bool BuildingMode;
    private bool isGathering;
    public bool IsGathering => isGathering;
    public InventoryResourceType LastGatheredResource;

    private float baseMiningDuration;
    private float miningDuration;
    public float MiningDuration => miningDuration;
    private int depositDuration = 350; //en milisecondes
    public int DepositDuration => depositDuration;

    private float timer;

    [Header("Inventory Management")]
    [SerializeField] private UnitInventory inventory;
    public UnitInventory Inventory => inventory;


    [Header("Animations & graphics")]
    [SerializeField] GameObject bag;
    [SerializeField] GameObject bagContainer;
    private Vector3 startingBagScale;
    [SerializeField] private Slider miningSlider;


    [Header("Building assignation")]

    public bool isAssignedToABuilding;
    public BuildingManager buildingAssigned;
    public WorkerAIUse workerAIUse;
    public ResourceSpotUI resourceAssigned;

    public bool canBeMovedByPlayer;

    [Header("Item transfer")]

    public bool isTransferingItems;
    protected override void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        inventory = gameObject.GetComponent<UnitInventory>();
        resourceToGather = 1;
        startingBagScale = bag.transform.localScale;
        bagContainer.SetActive(false);
        workerAIUse = gameObject.GetComponentInChildren<WorkerAIUse>();
        workerAIC = gameObject.GetComponentInChildren<WorkerAIC>();
        workerBehaviour = gameObject.GetComponentInChildren<WorkerBehaviour>();


        base.Awake();
        baseMiningDuration = unitData.gatheringTime;
        miningDuration = baseMiningDuration;
        timer = miningDuration;
        miningSlider.maxValue = timer;
        miningSlider.value = 0;
        miningSlider.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (isGathering && workerAIC.CurrentBehaviour == workerAIUse)
        {
            if (inventory.InventorySystem.HasFreeSlot(out InventorySlot _freeSlot))
            {
                animator.SetBool("Walking", false);
                animator.SetBool("Interaction", true);

                miningSlider.gameObject.SetActive(true);
                timer -= Time.deltaTime;
                miningSlider.value = miningSlider.maxValue - timer;
                if (timer <= 0f)
                {
                    if (resourceSpot.Quantity > 0)
                    {
                        
                        resourceSpot.GatherResources(resourceToGather);

                        if (buildingAssigned == null) 
                        {
                            HealthPoints -= 2;
                            HealthUpdate();
                            miningDuration = baseMiningDuration * 2;
                        }
                        else
                        {
                            miningDuration = baseMiningDuration;
                        }
                        inventory.InventorySystem.AddToInventory(item.item, resourceToGather);
                        LastGatheredResource = item.item.resourceType;
                        
                        //change the bag according to how much there is inside
                        ShowBag();
                        ChangeBagSize(CalculateBagSize());

                        //Dynamic display of character inventory if he is selected
                        DisplayThisIventory();
                    }

                    timer = miningDuration;
                }
            }
            else
            {
                ExitGatheringMode();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInChildren<ResourceSpot>() != null && GatheringMode == true && inventory.InventorySystem.HasFreeSlot(out InventorySlot _freeSlot))
        {
            resourceSpot = other.gameObject.GetComponentInChildren<ResourceSpot>();
            item = other.gameObject.GetComponentInChildren<ItemRef>();
            StartGathering();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInChildren<ResourceSpot>() != null)
            StopGathering();
    }

    public void StartGathering() //make the unit start gathering resources
    {
        isGathering = true;
        timer = miningDuration;
    }

    public void StopGathering() //make the unit stop gathering resources
    {
        animator.SetBool("Interaction", false);
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
        if (inventory.InventorySystem.KnowIfInventoryIsEmpty())
        {
            bagContainer.SetActive(false);
        }

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

        float ratio = _itemsInBag / _totalItemsInBag * 100;

        if (ratio < 34) _bagSize = 1;
        else if (ratio < 67) _bagSize = 2;
        else _bagSize = 3;

        return _bagSize;
    }

    public void ChangeBagSize(int _bagSize)
    {
        switch (_bagSize)
        {
            case 1:
                bag.transform.localScale = startingBagScale; break;
            case 2:
                bag.transform.localScale = new Vector3(startingBagScale.x * 1.5f, startingBagScale.y * 1.5f, startingBagScale.z * 1.5f); break;
            case 3:
                bag.transform.localScale = new Vector3(startingBagScale.x * 2f, startingBagScale.y * 2f, startingBagScale.z * 2f); break;
            default:
                bag.transform.localScale = startingBagScale; break;
        }
    }

    //Dynamic display of character inventory if he is selected
    public void DisplayThisIventory()
    {
        if (Global.SELECTED_CHARACTERS.Count == 1 && Global.SELECTED_CHARACTERS[0] == gameObject.GetComponent<UnitManager>())
        {
            ShowInventoryUI(inventory.InventorySystem);
        }
    }
}
