using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Transform buildingMenu; // A reference to the UI game object that holds building buttons
    [SerializeField] private GameObject buildingButtonPrefab; // Prefab for building menu buttons

    private BuildingPlacer buildingPlacer; // Reference to the BuildingPlacer script

    private void Awake()
    {
        buildingPlacer = GetComponent<BuildingPlacer>(); // Get the BuildingPlacer component of the same GameObject

        // Create buttons for each building type
        for (int i = 0; i < Global.BUILDING_DATA.Length; i++)
        {
            // Instantiate a button prefab for each building type
            GameObject button = GameObject.Instantiate(buildingButtonPrefab, buildingMenu);

            // Set the name of the button to the building code for identification
            string code = Global.BUILDING_DATA[i].Code;
            button.name = code;

            // Set the button's text to the building code
            button.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = code;

            Button b = button.GetComponent<Button>();

            // Add a listener to the button to handle the selection of the building type
            AddBuildingButtonListener(b, i);
        }
    }

    // Add a listener to a building button
    private void AddBuildingButtonListener(Button b, int i)
    {
        // Use a lambda expression to add a listener that selects the corresponding building type
        b.onClick.AddListener(() => buildingPlacer.SelectPlacedBuilding(i));
    }
}
