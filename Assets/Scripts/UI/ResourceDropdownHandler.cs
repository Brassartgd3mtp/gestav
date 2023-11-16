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

    private InventoryItemData currentlyAssociatedData;
    public InventoryItemData CurrentlyAssociatedData => currentlyAssociatedData;

    private Dictionary<TMP_Dropdown.OptionData, InventoryItemData> optionsReferences = new Dictionary<TMP_Dropdown.OptionData, InventoryItemData>();
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
        optionsReferences.Clear();

        // Create a list of options based on totalItemCounts

        List<string> dropDownOptions = new List<string>();

        foreach (var kvp in gameResourceManager.totalItemCount)
        {
            string key = $"{kvp.Key.DisplayName} : {kvp.Value}";
            InventoryItemData value = kvp.Key;

            TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData(key);
            optionsReferences.Add(optionData, value);

            dropDownOptions.Add(key);
        }
        // Add options to the dropdown
        dropdown.AddOptions(dropDownOptions);
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

        if (optionsReferences.TryGetValue(selectedOption, out InventoryItemData associatedData))
        {
            currentlySelectedOption = selectedOption;
            currentlyAssociatedData = associatedData;
        }
    }
}
