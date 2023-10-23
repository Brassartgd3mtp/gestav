using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourceSpotUI : MonoBehaviour
{
    public GameObject PopUpPanel;
    public TextMeshProUGUI QuantityText;
    public ResourceSpot Resource;
    public Material OutilineMaterial;

    private void Awake()
    {
        PopUpPanel.gameObject.SetActive(false);

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

    public void UpdateResourceQuantity()
    {
        QuantityText.text = Resource.Quantity.ToString();
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
}
