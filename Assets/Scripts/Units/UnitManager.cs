using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class UnitManager : MonoBehaviour
{
    public Material OutilineMaterial;

    protected BoxCollider _collider;
    private BuildingActionSelection buildingActionSelection;

    private void Awake()
    {
        buildingActionSelection = GetComponentInChildren<BuildingActionSelection>();
    }

    protected virtual Unit Unit { get; set; }

    // Called when the mouse is clicked on the unit
    private void OnMouseDown()
    {
        if (IsActive())
            Select(true, Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
    }

    // Check if the unit is active and can be selected
    protected virtual bool IsActive()
    {
        return true;
    }

    // Utility method for selecting the unit
    private void SelectUtil()
    {
        if (Global.SELECTED_UNITS.Contains(this)) return;
        Global.SELECTED_UNITS.Add(this);

        AddMaterial(OutilineMaterial);
    }

    // Select the unit, allowing for multiple selections with or without the Shift key
    public void Select()
    {
        Select(false, false);
    }

    public void Select(bool _singleClick, bool _holdingShift)
    {
        // Basic case: using the selection box
        if (!_singleClick)
        {
            SelectUtil();
            return;
        }

        // Single click: check for Shift key
        if (!_holdingShift)
        {
            List<UnitManager> selectedUnits = new List<UnitManager>(Global.SELECTED_UNITS);
            foreach (UnitManager um in selectedUnits)

                    um.Deselect();
                    SelectUtil();


        }
        else
        {
            if (!Global.SELECTED_UNITS.Contains(this))
                SelectUtil();
            else
                Deselect();
        }
    }

    // Deselect the unit
    public void Deselect()
    {
        if (!Global.SELECTED_UNITS.Contains(this)) return;
        Global.SELECTED_UNITS.Remove(this);

        RemoveMaterial("M_Outline (Instance)");

        if(buildingActionSelection != null)
        {
            buildingActionSelection.TransferPanel.SetActive(false);
        }
    }

    public void Initialize(Unit _unit)
    {
        _collider = GetComponent<BoxCollider>();
        Unit = _unit;
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
