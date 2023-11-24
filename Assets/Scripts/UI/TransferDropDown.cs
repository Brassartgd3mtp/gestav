using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class TransferDropDown : MonoBehaviour
{
    private TMP_Dropdown dropdown;
    private BuildingInventory inventory;
    private TMP_Dropdown.OptionData currentlySelectedOption;
    public TMP_Dropdown.OptionData CurrentlySelectedOption => currentlySelectedOption;

    private BuildingInventory currentlyAssociatedData;
    public BuildingInventory CurrentlyAssociatedData => currentlyAssociatedData;



    private Dictionary<string, BuildingInventory> optionsReferences = new Dictionary<string, BuildingInventory>();

    private void Awake()
    {
        inventory = GetComponentInParent<BuildingInventory>();
        dropdown = GetComponent<TMP_Dropdown>();

        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        UpdateDropDown();

        TMP_Dropdown.OptionData selectedOption = dropdown.options[0];

        if (optionsReferences.TryGetValue(selectedOption.text, out BuildingInventory associatedData))
        {
            currentlyAssociatedData = associatedData;
        }

        currentlySelectedOption = selectedOption;
    }

    public void PopulateDropDown()
    {
        // Clear existing options
        dropdown.ClearOptions();
        List<string> dropdownOptions = new List<string>();
        List<BuildingInventory> buildingInventories = new List<BuildingInventory>();

        optionsReferences.Clear();

        foreach (BuildingInventory building in Global.BUILDINGS)
        {
            string option;
                if(building == inventory) 
                {
                    option = $"HERE {building.InventorySystem.InventorySize - building.InventorySystem.AmountOfSlotsAvaliable()}/{building.InventorySystem.InventorySize}";
                }
                else if(building.GetComponent<Extractioninventory>() != null)
                {
                    option = $"EXTRACTION ({Mathf.RoundToInt(building.transform.position.x)};{Mathf.RoundToInt(building.transform.position.z)})";
                }
                else
                {
                    option = $"{building.gameObject.name} {building.InventorySystem.InventorySize - building.InventorySystem.AmountOfSlotsAvaliable()}/{building.InventorySystem.InventorySize} ({Mathf.RoundToInt(building.transform.position.x)};{Mathf.RoundToInt(building.transform.position.z)})";
                }
            dropdownOptions.Add(option);
            buildingInventories.Add(building);

        }
        // Add options to the dropdown
        dropdown.AddOptions(dropdownOptions);

        for (int i = 0; i < dropdownOptions.Count; i++)
        {
            optionsReferences.Add(dropdownOptions[i], buildingInventories[i]);
        }
        Debug.Log(dropdown.options);
    }

    private void OnDropdownValueChanged(int index)
    {
        TMP_Dropdown.OptionData selectedOption = dropdown.options[index];

        if (optionsReferences.TryGetValue(selectedOption.text, out BuildingInventory associatedData))
            {
            currentlyAssociatedData = associatedData;
            }

        currentlySelectedOption = selectedOption;
    }


        public void UpdateDropDown()
    {
        PopulateDropDown();
    }
}
