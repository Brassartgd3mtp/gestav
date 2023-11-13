using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CharacterUI characterUI;


    [SerializeField] private Transform buildingMenu; // A reference to the UI game object that holds building buttons
    [SerializeField] private GameObject buildingButtonPrefab; // Prefab for building menu buttons

    private BuildingPlacer buildingPlacer; // Reference to the BuildingPlacer script
    private Dictionary<string, Button> buildingButtons; // Dictionary to hold the building buttons

    private void Awake()
    {
        buildingPlacer = GetComponent<BuildingPlacer>(); // Get the BuildingPlacer component of the same GameObject


        // Create buttons for each building type

        buildingButtons = new Dictionary<string, Button>();

        for (int i = 0; i < Global.BUILDING_DATA.Length; i++)
        {


            BuildingData data = Global.BUILDING_DATA[i];
            GameObject button = Instantiate(buildingButtonPrefab, buildingMenu);
            button.name = data.unitName;
            button.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = data.unitName;
            Button b = button.GetComponent<Button>();
            buildingButtons[data.code] = b;


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

    private void Update()
    {
        if(Global.SELECTED_UNITS.Count > 0)
        {
            characterUI.ShowWorkerUI();
        }
        else 
        { 
            characterUI.HideWorkerUI(); 
        }
    }


}