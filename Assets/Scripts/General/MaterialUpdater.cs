using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialUpdater : MonoBehaviour
{
    public void AddMaterial(Material material)
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

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
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

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
