using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Threading.Tasks;
using TMPro;
using JetBrains.Annotations;

public class Extractioninventory : MonoBehaviour
{
    private UnitInventory workerInventory;
    private BuildingInventory thisInventory;
    private int resourcesExtracted = 0;
    private int amountToExtract;
    //private bool amountExctracted;
    [SerializeField] private GameObject VictoryUI;
    [SerializeField] private TextMeshProUGUI currentResourcesText;
    [SerializeField] private TextMeshProUGUI totalResourcesText;


    //private int charactersInZone = 0;
    private List<InventoryHolder> workerInventoryList = new List<InventoryHolder>();

    private void Awake()
    {
        thisInventory = GetComponent<BuildingInventory>();
        //amountExctracted = false;
        amountToExtract = 10;
        VictoryUI.SetActive(false);
        totalResourcesText.text = amountToExtract.ToString();
        currentResourcesText.text = resourcesExtracted.ToString();
    }

    public void UpdateInventoryInfo()
    {
        resourcesExtracted = thisInventory.InventorySystem.InventorySlots.Count - thisInventory.InventorySystem.AmountOfSlotsAvaliable();
        currentResourcesText.text = resourcesExtracted.ToString();
        if (resourcesExtracted >= amountToExtract)
        {
            //amountExctracted = true;
            VictoryUI.SetActive(true);
        }
        
    }

    public void UpdateaSlotsAmount()
    {
        foreach (InventorySlot slot in thisInventory.InventorySystem.InventorySlots)
        {
            if (slot.ItemData == null)
            {
                thisInventory.InventorySystem.InventorySlots.Remove(slot);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out UnitInventory inv))
        {
            for (int i = 0; i < inv.InventorySystem.InventorySize; i++)
            {
                if (inv.InventorySystem.InventorySlots[i].ItemData != null)
                {
                    thisInventory.InventorySystem.InventorySlots.Add(new InventorySlot());
                    thisInventory.InventorySystem.AddToInventory(inv.InventorySystem.InventorySlots[i].ItemData, 1);
                    inv.InventorySystem.InventorySlots[i].ClearSlot();
                    UpdateInventoryInfo();
                }
            }
            UpdateaSlotsAmount();

        }
    }

}
