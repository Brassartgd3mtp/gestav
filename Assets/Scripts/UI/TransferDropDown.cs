using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TransferDropDown : MonoBehaviour
{
    private TMP_Dropdown dropdown;
    private BuildingInventory inventory;
    private void Awake()
    {
        inventory = GetComponentInParent<BuildingInventory>();
        dropdown = GetComponent<TMP_Dropdown>();
        UpdateDropDown();
    }

    public void PopulateDropDown()
    {
        // Clear existing options
        dropdown.ClearOptions();
        List<string> dropdownOptions = new List<string>();
        foreach (var building in Global.BUILDINGS)
        {
                if(building == inventory) 
                {
                    dropdownOptions.Add($"HERE {building.InventorySystem.InventorySize - building.InventorySystem.AmountOfSlotsAvaliable()}/{building.InventorySystem.InventorySize}");
                }
                else if(building.GetComponent<Extractioninventory>() != null)
                {
                    dropdownOptions.Add($"EXTRACTION ({Mathf.RoundToInt(building.transform.position.x)};{Mathf.RoundToInt(building.transform.position.z)})");
                }
                else
                {
                    dropdownOptions.Add($"{building.gameObject.name} {building.InventorySystem.InventorySize - building.InventorySystem.AmountOfSlotsAvaliable()}/{building.InventorySystem.InventorySize} ({Mathf.RoundToInt(building.transform.position.x)};{Mathf.RoundToInt(building.transform.position.z)})");
                }
        
        }
        // Add options to the dropdown
        dropdown.AddOptions(dropdownOptions);
    }

    public void UpdateDropDown()
    {
        PopulateDropDown();
    }
}
