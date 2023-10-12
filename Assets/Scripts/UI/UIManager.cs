using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Transform buildingMenu; // holds a reference a UI game object that is a child of the Canvas and is the parent of all the building buttons
    [SerializeField] private GameObject buildingButtonPrefab; //is linked to a prefab of a UI button customised for the building menu

    private BuildingPlacer buildingPlacer;

    private void Awake()
    {
        buildingPlacer = GetComponent<BuildingPlacer>();

        // create buttons for each building type
        for (int i = 0; i < Global.BUILDING_DATA.Length; i++) // loop through all of the types in our global list and instantiate the newly create button prefab as many times as necessary
        {
            GameObject button = GameObject.Instantiate(
                buildingButtonPrefab,
                buildingMenu);
            string code = Global.BUILDING_DATA[i].Code;
            button.name = code;
            button.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = code;
            Button b = button.GetComponent<Button>();
            AddBuildingButtonListener(b, i);
        }
    }

    private void AddBuildingButtonListener(Button b, int i)
    {
        b.onClick.AddListener(() => buildingPlacer.SelectPlacedBuilding(i));
    }

}
