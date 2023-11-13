using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuildingStockageUI : MonoBehaviour
{
    public GameObject PopUpPanel;
    public TextMeshProUGUI MaxQuantityText;
    public TextMeshProUGUI CurrentQuantityText;
    public InventoryHolder Inventory;
    public Material OutilineMaterial;

    private void Awake()
    {
        PopUpPanel.gameObject.SetActive(false);
        Inventory = GetComponent<InventoryHolder>();
        UpdateSpaceInUI();
    }

    private void OnMouseEnter()
    {
        PopUpPanel.gameObject.SetActive(true);
        AddMaterial(OutilineMaterial);

        foreach (GameObject go in GetAllWorkersAssigned())
        {
            Debug.Log(go);
            CharacterManager characterManagerRef = go.GetComponent<CharacterManager>();
            BuildingManager buildingManager = gameObject.GetComponent<BuildingManager>();
            if (characterManagerRef != null && characterManagerRef.buildingAssigned == buildingManager)
            {
                Blink blink = go.GetComponentInChildren<Blink>();
                blink.StartBlinking();
            }
        }

    }

    private void OnMouseExit()
    {
        PopUpPanel.gameObject.SetActive(false);
        RemoveMaterial("M_Outline (Instance)");

        foreach (GameObject go in GetAllWorkersAssigned())
        {
            CharacterManager characterManagerRef = go.GetComponent<CharacterManager>();
            BuildingManager buildingManager = gameObject.GetComponent<BuildingManager>();
            if (characterManagerRef != null && characterManagerRef.buildingAssigned == buildingManager)
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
        if(Inventory.InventorySystem.AmountOfSlotsAvaliable() == 0)
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
            // Récupère les matériaux actuels
            List<Material> materialList = new List<Material>(meshRenderer.materials);

            // Ajoute le nouveau matériau à la liste des matériaux
            materialList.Add(material);

            // Applique la nouvelle liste de matériaux au MeshRenderer
            meshRenderer.materials = materialList.ToArray();
        }
    }

    public void RemoveMaterial(string materialName)
    {
        MeshRenderer meshRenderer = gameObject.GetComponentInChildren<MeshRenderer>();

        if (meshRenderer != null)
        {
            // Récupère les matériaux actuels
            List<Material> materialList = new List<Material>(meshRenderer.materials);

            // Recherche et enlève le matériau spécifié de la liste par nom
            Material materialToRemove = materialList.Find(m => m.name == materialName);

            if (materialToRemove != null)
            {
                materialList.Remove(materialToRemove);

                // Applique la nouvelle liste de matériaux au MeshRenderer
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

        Debug.Log(go);
        return go;

    }
}

