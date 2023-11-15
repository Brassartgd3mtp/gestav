using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TransferDropDown : MonoBehaviour
{
    private TMP_Dropdown dropdown;
    private BuildingInventory inventory;
    private TMP_Dropdown.OptionData currentlySelectedOption;
    public TMP_Dropdown.OptionData CurrentlySelectedOption => currentlySelectedOption;

    private BuildingInventory currentlyAssociatedData;
    public BuildingInventory CurrentlyAssociatedData => currentlyAssociatedData;


    private Dictionary<TMP_Dropdown.OptionData, BuildingInventory> optionsReferences = new Dictionary<TMP_Dropdown.OptionData, BuildingInventory>();

    private void Awake()
    {
        inventory = GetComponentInParent<BuildingInventory>();
        dropdown = GetComponent<TMP_Dropdown>();

        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        UpdateDropDown();
    }

    public void PopulateDropDown()
    {
        // Clear existing options
        dropdown.ClearOptions();
        List<string> dropdownOptions = new List<string>();


        foreach (var building in Global.BUILDINGS)
        {
            string key;
                if(building == inventory) 
                {
                    key = $"HERE {building.InventorySystem.InventorySize - building.InventorySystem.AmountOfSlotsAvaliable()}/{building.InventorySystem.InventorySize}";
                }
                else if(building.GetComponent<Extractioninventory>() != null)
                {
                    key = $"EXTRACTION ({Mathf.RoundToInt(building.transform.position.x)};{Mathf.RoundToInt(building.transform.position.z)})";
                }
                else
                {
                    key = $"{building.gameObject.name} {building.InventorySystem.InventorySize - building.InventorySystem.AmountOfSlotsAvaliable()}/{building.InventorySystem.InventorySize} ({Mathf.RoundToInt(building.transform.position.x)};{Mathf.RoundToInt(building.transform.position.z)})";
                }
            dropdownOptions.Add(key);
            BuildingInventory value = building;
            TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData(key);
            optionsReferences.Add(optionData, value);

        }
        // Add options to the dropdown
        dropdown.AddOptions(dropdownOptions);
    }

    private void OnDropdownValueChanged(int index)
    {
        // This function is called whenever the selected option in the dropdown changes
        // 'index' represents the index of the selected option in the dropdown's options list

        // You can access the selected option's text like this:
        TMP_Dropdown.OptionData selectedOption = dropdown.options[index];

        if (optionsReferences.TryGetValue(selectedOption, out BuildingInventory associatedData))
        {
            // Now you can use 'associatedData' in your code
            Debug.Log("Selected option: " + selectedOption.text);
            Debug.Log("Associated InventoryItemData: " + associatedData);
        }

        currentlySelectedOption = selectedOption;
        currentlyAssociatedData = associatedData;
    }

        public void UpdateDropDown()
    {
        PopulateDropDown();
    }
}
