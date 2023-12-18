using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUnitManager : MonoBehaviour
{

    [Header("Statistics")]
    public int HealthPoints;

    public EnemyData EnemyData;

    [Header("Animations & graphics")]
    [SerializeField] private Slider healthBar;
    [SerializeField] protected TextMeshProUGUI currentHPText;
    [SerializeField] protected TextMeshProUGUI maxHPText;

    [SerializeField] private Material outlineMaterial;


    protected virtual void Awake()
    {
        healthBar.maxValue = EnemyData.healthPoints;
        HealthPoints = EnemyData.healthPoints;
        maxHPText.text = healthBar.maxValue.ToString();
        HealthUpdate();
    }

    protected void FixedUpdate()
    {
        HealthUpdate();
    }

    public virtual void HealthUpdate()
    {
        healthBar.value = HealthPoints;
        currentHPText.text = healthBar.value.ToString();
        if (HealthPoints <= 0)
        {
            Destroy(gameObject);
        }
        if (HealthPoints == EnemyData.healthPoints)
        {
            healthBar.gameObject.SetActive(false);
        }
        else
        {
            healthBar.gameObject.SetActive(true);
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
