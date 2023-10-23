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
