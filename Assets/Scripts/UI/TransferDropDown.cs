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
            TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData(option);
            BuildingInventory value = building;

            optionsReferences.Add(optionData, value);

        }
        // Add options to the dropdown
        dropdown.AddOptions(dropdownOptions);
        Debug.Log(dropdown.options);
    }

    private void OnDropdownValueChanged(int index)
    {

        Debug.Log("Options in dictionary:");
        foreach (var kvp in optionsReferences)
        {
            Debug.Log($"{kvp.Key.text} => {kvp.Value}");
        }

        TMP_Dropdown.OptionData selectedOption = dropdown.options[index];

        Debug.Log("Selected option index: " + index);
        Debug.Log("Selected option text: " + selectedOption.text);

        if (optionsReferences.TryGetValue(selectedOption, out BuildingInventory associatedData))
            {
            currentlyAssociatedData = associatedData;
            Debug.Log("Selected data " + associatedData);
            }


            currentlySelectedOption = selectedOption;
            

    }

        public void UpdateDropDown()
    {
        PopulateDropDown();
    }
}
