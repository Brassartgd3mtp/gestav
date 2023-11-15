using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ResourceDropdownHandler : MonoBehaviour
{
    private TMP_Dropdown dropdown;
    private GameResourceManager gameResourceManager;
    private TMP_Dropdown.OptionData currentlySelectedOption;
    public TMP_Dropdown.OptionData CurrentlySelectedOption => currentlySelectedOption;

    private void Awake()
    {


        dropdown = GetComponent<TMP_Dropdown>();
        gameResourceManager = FindObjectOfType<GameResourceManager>();

        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        UpdateDropdown();
    }
    public void PopulateDropdown()
    {
        // Clear existing options
        dropdown.ClearOptions();

        // Create a list of options based on totalItemCounts
        Dictionary<string, InventoryItemData> dropDownOptions = new Dictionary<string, InventoryItemData>();
        // List<string> dropdownOptions = new List<string>();
        foreach (var kvp in gameResourceManager.totalItemCount)
        {
            string key = $"{kvp.Key.DisplayName} : {kvp.Value}";
            InventoryItemData value = kvp.Key;
            dropDownOptions.Add(key, value);
            // dropdownOptions.Add($"{kvp.Key.DisplayName} : {kvp.Value}" );
        }

        // Add options to the dropdown
        dropdown.AddOptions(dropDownOptions.Keys.ToListPooled());
    }

    public void UpdateDropdown()
    {
        PopulateDropdown();
    }

    private void OnDropdownValueChanged(int index)
    {
        // This function is called whenever the selected option in the dropdown changes
        // 'index' represents the index of the selected option in the dropdown's options list

        // You can access the selected option's text like this:
        TMP_Dropdown.OptionData selectedOption = dropdown.options[index];

        currentlySelectedOption = selectedOption;
        // Now you can use 'selectedOption' in your code
        Debug.Log("Selected option: " + selectedOption);
    }


}
