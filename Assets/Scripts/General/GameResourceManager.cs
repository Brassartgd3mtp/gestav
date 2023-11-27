using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResourceManager : MonoBehaviour
{
    private UIManager uiManager;


    private float updateTimer;
    private float updateInterval = 1f; 
    public Dictionary<InventoryItemData, int> TotalItemCount = new Dictionary<InventoryItemData, int>();

    private void Awake()
    {
        uiManager = GetComponent<UIManager>();
    }


    public Dictionary<InventoryItemData, int> SumItemCountsAcrossInventories(List<UnitInventory> unitsInventories)
    {
        Dictionary<InventoryItemData, int> itemCounts = new Dictionary<InventoryItemData, int>();

        foreach (UnitInventory inventory in unitsInventories)
        {
            if (inventory.InventorySystem != null) // Check for null InventorySystem
            {
                foreach (InventorySlot slot in inventory.InventorySystem.InventorySlots)
                {
                    if (slot != null && slot.ItemData != null) // Check for null InventorySlot and ItemData
                    {
                        if (itemCounts.ContainsKey(slot.ItemData))
                        {
                            itemCounts[slot.ItemData] += 1;
                            Debug.Log(itemCounts);
                        }
                        else
                        {
                            itemCounts.Add(slot.ItemData, 1);
                            Debug.Log(itemCounts);
                        }
                    }
                }
            }
        }
        return itemCounts;
    }



    public List<UnitInventory> FindInventories()
    {
        Global.allInventories.Clear();

        float detectionRadius = 10000f;
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, Global.UNIT_LAYER_MASK);
        List<UnitInventory> unitsInventories = new List<UnitInventory>();
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.GetComponent<UnitInventory>() != null)
            {
                unitsInventories.Add(colliders[i].gameObject.GetComponent<UnitInventory>());
            }
        }
        Global.allInventories = unitsInventories;
        Debug.Log(Global.allInventories);
        return Global.allInventories;
    }



    public void GetTotalItems()
    {
        TotalItemCount.Clear();
        Dictionary<InventoryItemData, int> _totalItemCounts = SumItemCountsAcrossInventories(Global.allInventories);

        // Access the total counts as needed
        foreach (var kvp in _totalItemCounts)
        {
            Debug.Log($"{kvp.Key}: {kvp.Value}");
        }
        TotalItemCount = _totalItemCounts;
        uiManager.UpdateResourceTexts(uiManager.CopperItemData, uiManager.CopperTMPro);
    }

    private void Update()
    {
        updateTimer += Time.deltaTime;
        if (updateTimer >= updateInterval)
        {
            updateTimer += Time.deltaTime;
            GetTotalItems();
            updateTimer = 0f;   
        }

    }
}