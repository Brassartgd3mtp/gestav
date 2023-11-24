using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Threading.Tasks;
using TMPro;
using JetBrains.Annotations;

public class Extractioninventory : MonoBehaviour
{
    private InventoryHolder workerInventory;
    private InventoryHolder thisInventory;
    private CharacterManager characterManager;
    private int resourcesExtracted = 0;
    private int amountToExtract;
    private bool amountExctracted;
    [SerializeField] private GameObject VictoryUI;
    [SerializeField] private TextMeshProUGUI currentResourcesText;
    [SerializeField] private TextMeshProUGUI totalResourcesText;


    private int charactersInZone = 0;
    private List<InventoryHolder> workerInventoryList = new List<InventoryHolder>();

    private void Awake()
    {
        thisInventory = GetComponent<InventoryHolder>();
        amountExctracted = false;
        amountToExtract = 5;
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
            amountExctracted = true;
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
}
