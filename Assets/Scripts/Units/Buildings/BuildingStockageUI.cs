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
    }

    private void OnMouseExit()
    {
        PopUpPanel.gameObject.SetActive(false);
        RemoveMaterial("M_Outline (Instance)");
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
}

