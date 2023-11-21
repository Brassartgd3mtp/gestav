using System;
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

    private Dictionary<string, InventoryItemData> optionsReferences = new Dictionary<string, InventoryItemData>();
    private void Awake()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        gameResourceManager = FindObjectOfType<GameResourceManager>();

        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        UpdateDropdown();
    }
    public void PopulateDropdown()
    {
        dropdown.ClearOptions();

        List<string> dropDownOptions = new List<string>();
        List<InventoryItemData> inventoryItems = new List<InventoryItemData>();

        optionsReferences.Clear();

        foreach (var kvp in gameResourceManager.totalItemCount)
        {
            string key = $"{kvp.Key.DisplayName} : {kvp.Value}";
            InventoryItemData value = kvp.Key;

            dropDownOptions.Add(key);
            inventoryItems.Add(value);
        }
        // Add options to the dropdown
        dropdown.AddOptions(dropDownOptions);

        for (int i = 0; i < dropDownOptions.Count; i++)
        {
            optionsReferences.Add(dropDownOptions[i], inventoryItems[i]);
        }
    }

    public void UpdateDropdown()
    {
        PopulateDropdown();
        if(optionsReferences.Count == 1)
        {
            TMP_Dropdown.OptionData selectedOption = dropdown.options[0];

            if (optionsReferences.TryGetValue(selectedOption.text, out InventoryItemData associatedData))
            {
                currentlyAssociatedData = associatedData;
            }
            currentlySelectedOption = selectedOption;

            Debug.Log(currentlyAssociatedData.DisplayName);
        }
    }

    private void OnDropdownValueChanged(int index)
    {
        TMP_Dropdown.OptionData selectedOption = dropdown.options[index];

        if (optionsReferences.TryGetValue(selectedOption.text, out InventoryItemData associatedData))
        {
            currentlyAssociatedData = associatedData;
        }
        currentlySelectedOption = selectedOption;

        Debug.Log(currentlyAssociatedData.DisplayName);
    }
}
