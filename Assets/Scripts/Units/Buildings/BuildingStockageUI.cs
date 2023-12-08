using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingStockageUI : MonoBehaviour
{
    [Header("Batiment construit")]

    public GameObject PopUpPanel;
    public TextMeshProUGUI MaxQuantityText;
    public TextMeshProUGUI CurrentQuantityText;

    [Header("Batiment en cours de construction")]

    public GameObject BuildProgresionPanel;
    public TextMeshProUGUI MaxQuantityNecessaryText;
    public TextMeshProUGUI CurrentQuantityGivenText;

    [Header("Overall")]

    public InventoryHolder Inventory;
    private ConstructionInventory constructionInventory;
    public Material OutilineMaterial;

    private BuildingManager buildingManager;
    public UnitData buildingData;
    [SerializeField] private Slider healthBar;

    private void Awake()
    {
        buildingManager = GetComponent<BuildingManager>();

        PopUpPanel.gameObject.SetActive(false);
        BuildProgresionPanel.gameObject.SetActive(false);

        Inventory = GetComponent<BuildingInventory>();
        constructionInventory = GetComponent<ConstructionInventory>();
        UpdateSpaceInUI();
    }
    private void Start()
    {
        UpdateSpaceInUI();
    }


    private void OnMouseEnter()
    {
        if (buildingManager.hasBeenBuilt)
        {
            PopUpPanel.gameObject.SetActive(true);
            BuildProgresionPanel.gameObject.SetActive(false);
        }
        else
        {
            PopUpPanel.gameObject.SetActive(false);
            BuildProgresionPanel.gameObject.SetActive(true);
        }
        AddMaterial(OutilineMaterial);

        foreach (GameObject go in GetAllWorkersAssigned())
        {
            WorkerManager workerManagerRef = go.GetComponent<WorkerManager>();
            BuildingManager buildingManager = gameObject.GetComponent<BuildingManager>();
            if (workerManagerRef != null && workerManagerRef.buildingAssigned == buildingManager)
            {
                Blink blink = go.GetComponentInChildren<Blink>();
                blink.StartBlinking();
            }
        }

    }

    private void OnMouseExit()
    {
        if(BuildProgresionPanel.activeSelf) BuildProgresionPanel.gameObject.SetActive(false);
        if(PopUpPanel.activeSelf) PopUpPanel.gameObject.SetActive(false);

        RemoveMaterial("M_Outline (Instance)");

        foreach (GameObject go in GetAllWorkersAssigned())
        {
            WorkerManager workerManagerRef = go.GetComponent<WorkerManager>();
            BuildingManager buildingManager = gameObject.GetComponent<BuildingManager>();
            if (workerManagerRef != null && workerManagerRef.buildingAssigned == buildingManager)
            {
                Blink blink = go.GetComponentInChildren<Blink>();
                blink.StopBlinking();
            }
        }
    }

    public void UpdateSpaceInUI()
    {
            MaxQuantityText.text = Inventory.InventorySystem.InventorySlots.Count.ToString();
            CurrentQuantityText.text = (Inventory.InventorySystem.InventorySlots.Count - Inventory.InventorySystem.AmountOfSlotsAvaliable()).ToString();


            MaxQuantityNecessaryText.text = constructionInventory.InventorySystem.InventorySize.ToString();
            CurrentQuantityGivenText.text = (constructionInventory.InventorySystem.InventorySize - constructionInventory.InventorySystem.AmountOfSlotsAvaliable()).ToString();

        if (Inventory.InventorySystem.AmountOfSlotsAvaliable() == 0)
            {
                MaxQuantityText.color = Color.red;
                CurrentQuantityText.color = Color.red;
            }
            else
            {
                MaxQuantityText.color = Color.white;
                CurrentQuantityText.color = Color.white;
            }
    }

    public void AddMaterial(Material material)
    {
        MeshRenderer meshRenderer = gameObject.GetComponentInChildren<MeshRenderer>();

        if (meshRenderer != null)
        {
            // R�cup�re les mat�riaux actuels
            List<Material> materialList = new List<Material>(meshRenderer.materials);

            // Ajoute le nouveau mat�riau � la liste des mat�riaux
            materialList.Add(material);

            // Applique la nouvelle liste de mat�riaux au MeshRenderer
            meshRenderer.materials = materialList.ToArray();
        }
    }

    public void RemoveMaterial(string materialName)
    {
        MeshRenderer meshRenderer = gameObject.GetComponentInChildren<MeshRenderer>();

        if (meshRenderer != null)
        {
            // R�cup�re les mat�riaux actuels
            List<Material> materialList = new List<Material>(meshRenderer.materials);

            // Recherche et enl�ve le mat�riau sp�cifi� de la liste par nom
            Material materialToRemove = materialList.Find(m => m.name == materialName);

            if (materialToRemove != null)
            {
                materialList.Remove(materialToRemove);

                // Applique la nouvelle liste de mat�riaux au MeshRenderer
                meshRenderer.materials = materialList.ToArray();
            }
        }
    }

    public GameObject[] GetAllWorkersAssigned()
    {
        float detectionRadius = 10000f;
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, Global.WORKER_LAYER_MASK);
        GameObject[] go = new GameObject[colliders.Length];

        for (int i = 0; i < colliders.Length; i++)
        {
            go[i] = colliders[i].gameObject;
        }
        return go;

    }
}

