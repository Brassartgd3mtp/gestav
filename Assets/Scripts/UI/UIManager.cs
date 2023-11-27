using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CameraControl;
using System.Linq;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CharacterUI characterUI;


    [SerializeField] private Transform buildingMenu; // A reference to the UI game object that holds building buttons
    [SerializeField] private GameObject buildingButtonPrefab; // Prefab for building menu buttons

    [SerializeField] private Transform workersMenu;
    [SerializeField] private GameObject workersButtonPrefab;

    private BuildingPlacer buildingPlacer; // Reference to the BuildingPlacer script
    private CameraMotion cameraMotion;
    private GameResourceManager gameResourceManager;

    private Dictionary<string, Button> buildingButtons; // Dictionary to hold the building buttons
    private Dictionary<Button, CharacterManager> workerButtons;

    [SerializeField] private Transform resourceMenu;
    [SerializeField] private GameObject resourceTextPrefab;

    [Header("Item Datas")]

    [SerializeField] private InventoryItemData copperItemData;
    public InventoryItemData CopperItemData => copperItemData;
    private TextMeshProUGUI copperTMPro;
    public TextMeshProUGUI CopperTMPro => copperTMPro;


    private void Awake()
    {
        buildingPlacer = GetComponent<BuildingPlacer>(); // Get the BuildingPlacer component of the same GameObject
        cameraMotion = FindAnyObjectByType<CameraMotion>();
        gameResourceManager = GetComponent<GameResourceManager>();
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


        workerButtons = new Dictionary<Button, CharacterManager>();
        CharacterManager[] workers = FindObjectsOfType<CharacterManager>();
        for (int i = 0;i<workers.Length;i++)
        {
            GameObject button = Instantiate(workersButtonPrefab, workersMenu);
            button.name = workers[i].name;
            button.transform.GetComponentInChildren<TextMeshProUGUI>().text = workers[i].name;
            Button b = button.GetComponent<Button>();
            AddWorkerButtonListener(b, workers[i]);
            workerButtons.Add(b, workers[i]);
        }

        GameObject copperText = Instantiate(resourceTextPrefab, resourceMenu);
        copperText.transform.GetComponent<TextMeshProUGUI>();
        copperTMPro = copperText.GetComponent<TextMeshProUGUI>();
        UpdateResourceTexts(copperItemData, copperTMPro);
    }

    // Add a listener to a building button
    private void AddBuildingButtonListener(Button b, int i)
    {
        // Use a lambda expression to add a listener that selects the corresponding building type
        b.onClick.AddListener(() => buildingPlacer.SelectPlacedBuilding(i));
    }

    public void AddWorkerButtonListener(Button b, CharacterManager characterManager)
    {
        b.onClick.AddListener(() => characterManager.Select(true, Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)));
        b.onClick.AddListener(() => cameraMotion.TargetPosition = new(characterManager.gameObject.transform.localPosition.x, characterManager.gameObject.transform.localPosition.y));
    }

    private void Update()
    {
        if(Global.SELECTED_CHARACTERS.Count > 0)
        {
            characterUI.ShowWorkerUI();
        }
        else 
        { 
            characterUI.HideWorkerUI(); 
        }
    }


    public void UpdateResourceTexts(InventoryItemData resourceData, TextMeshProUGUI resourcetext)
    {
        if (gameResourceManager.TotalItemCount.TryGetValue(resourceData, out int value))
        {
            resourcetext.text = $"Copper : {value}";
        }
        else
        {
            resourcetext.text = $"Copper : 0";
        }
    }
}