using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUnitManager : MonoBehaviour
{

    [Header("Statistics")]
    public int HealthPoints;

    public EnemyData EnemyData;

    [Header("Animations & graphics")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private Material outlineMaterial;


    protected virtual void Awake()
    {
        healthBar.maxValue = EnemyData.healthPoints;
        HealthPoints = EnemyData.healthPoints;
    }

    protected void FixedUpdate()
    {
        HealthUpdate();
    }

    public virtual void HealthUpdate()
    {
        healthBar.value = HealthPoints;
        if (HealthPoints <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnMouseEnter()
    {
        AddMaterial(outlineMaterial);
    }
    private void OnMouseExit()
    {
        RemoveMaterial("M_Outline_Enemy (Instance)");
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
