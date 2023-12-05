using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class CharacterManager : UnitManager
{
    [Header("Statistics")]
    public int HealthPoints;

    [Header("Scripts")]

    [SerializeField] private Find findingScript;
    public UnitData unitData;
    private Character character;
    private AssignWorkerInventory assignWorker;

    private ResourceSpot resourceSpot;
    public ResourceSpot ResourceSpot => resourceSpot;
    private ItemRef item;
    public ItemRef Item => item;


    [Header("Navigation")]

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float stoppingDistance;
    //[SerializeField] private float stoppingMultiplicator = 0.3f;


    [Header("IA")]

    private WorkerAIC workerAIC;

    [Header("Collect")]

    [SerializeField] private int resourceToGather = 1;
    public int ResourceToGather => resourceToGather;

    public bool GatheringMode;
    public bool BuildingMode;
    private bool isGathering;
    public bool IsGathering => isGathering;
    public InventoryResourceType LastGatheredResource;

    private float miningDuration = 3f;
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
    [SerializeField] private Slider healthBar;
    private Vector3 startingBagScale;

    public Animator animator;


    [Header("Building assignation")]

    public bool isAssignedToABuilding;
    public BuildingManager buildingAssigned;
    public WorkerAIUse workerAIUse;


    [Header("Item transfer")]

    public bool isTransferingItems;

    protected override Unit Unit
    {
        get { return character; }
        set { character = value is Character ? (Character)value : null; }
    }

    private void Awake()
    {
        inventory = gameObject.GetComponent<UnitInventory>();
        resourceToGather = 1;
        startingBagScale = bag.transform.localScale;
        bagContainer.SetActive(false);
        workerAIUse = gameObject.GetComponentInChildren<WorkerAIUse>();
        workerAIC = gameObject.GetComponentInChildren<WorkerAIC>();

        healthBar.maxValue = unitData.healthPoints;
        HealthPoints = unitData.healthPoints;
    }

    private void Update()
    {
        if(isGathering && workerAIC.CurrentBehaviour == workerAIUse)
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
                ExitGatheringMode(); // a bouger dans un endroit ou l'on check si l'inventaire est plein
            }
        }
    }

    private void FixedUpdate()
    {
        HealthUpdate();
    }

    private void HealthUpdate()
    {
        healthBar.value = HealthPoints;

        if (HealthPoints <= 0)
            Destroy(gameObject);
    }

    public async void MoveTo(Vector3 targetPosition, float _rangeToStop)
    {
        // Stop the current movement
        agent.isStopped = true;
        // Set the new destination
        agent.destination = targetPosition;

        // Resume movement
        agent.isStopped = false;
        while (agent != null && agent.velocity != Vector3.zero)
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
                if (inventories.Length > 0 && GetItemInFirstOccupiedSlot() != null && inv.validType.Contains(GetItemInFirstOccupiedSlot().resourceType))
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

       float ratio =  _itemsInBag / _totalItemsInBag * 100;

        if (ratio < 34) _bagSize = 1;
        else if (ratio < 67) _bagSize = 2;
        else _bagSize = 3;

        return _bagSize;
    }

    public void ChangeBagSize(int _bagSize)
    {
        switch(_bagSize) 
        {
        case 1:
                bag.transform.localScale = startingBagScale; break;
        case 2:
                bag.transform.localScale = new Vector3(startingBagScale.x *1.5f, startingBagScale.y * 1.5f, startingBagScale.z * 1.5f); break;
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


    // Utility method for selecting the unit
    protected override void SelectUtil()
    {
        base.SelectUtil();
        if (Global.SELECTED_CHARACTERS.Contains(this)) return;
        Global.SELECTED_CHARACTERS.Add(this);

        AddMaterial(OutilineMaterial);
    }

    // Select the unit, allowing for multiple selections with or without the Shift key
    public override void Select()
    {
        Select(false, false);
    }

    public override void Select(bool _singleClick, bool _holdingShift)
    {
        base.Select();
        // Basic case: using the selection box
        if (!_singleClick)
        {
            SelectUtil();
            return;
        }

        // Single click: check for Shift key
        if (!_holdingShift)
        {
            List<CharacterManager> selectedUnits = new List<CharacterManager>(Global.SELECTED_CHARACTERS);
            foreach (CharacterManager um in selectedUnits)

                um.Deselect();
            SelectUtil();


        }
        else
        {
            if (!Global.SELECTED_CHARACTERS.Contains(this))
                SelectUtil();
            else
                Deselect();
        }
    }

    // Deselect the unit
    public override void Deselect()
    {
        base.Deselect();
        if (!Global.SELECTED_CHARACTERS.Contains(this)) return;
        Global.SELECTED_CHARACTERS.Remove(this);

        RemoveMaterial("M_Outline (Instance)");
    }
}

