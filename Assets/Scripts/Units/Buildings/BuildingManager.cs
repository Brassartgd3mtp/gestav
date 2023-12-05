using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

[Flags]
public enum BuildingType
{
    Collect = 1 << 0,
    Crafting = 1 << 1
}
public class BuildingManager : UnitManager
{


    public BuildingType TypeOfBuilding;

    [SerializeField] private Building building;
    public Building Building => building;
    private GameResourceManager gameResourceManager;

    public bool hasBeenBuilt;

    protected override Unit Unit
    {
        get { return building; }
        set { building = value is Building ? (Building)value : null; }
    }

    private void Awake()
    {
        gameResourceManager = FindAnyObjectByType<GameResourceManager>();
        hasBeenBuilt = false;
    }

    private int nCollisions = 0;  // Counter for collision events

    // Called when another collider enters this object's trigger area
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Terrain") return;  // Ignore terrain collisions
        nCollisions++;  // Increase the collision count
        CheckPlacement();  // Check if the placement is still valid
    }

    // Called when another collider exits this object's trigger area
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Terrain") return;  // Ignore terrain collisions
        nCollisions--;  // Decrease the collision count
        CheckPlacement();  // Check if the placement is still valid
    }

    // Check if the current placement of the building is valid
    public bool CheckPlacement()
    {
        if (building == null) return false;  // If no building is assigned, return false
        if (building.IsFixed) return false;  // If the building is already fixed in place, return false

        bool _validPlacement = HasValidPlacement();  // Check if the placement is valid

        if (!_validPlacement)
        {
            building.SetMaterials(BuildingPlacement.INVALID);  // Set materials to indicate an invalid placement
        }
        else
        {
            building.SetMaterials(BuildingPlacement.VALID);  // Set materials to indicate a valid placement
        }

        return _validPlacement;  // Return whether the placement is valid or not
    }

    // Check if the placement of the building is valid
    public bool HasValidPlacement()
    {
        if (nCollisions > 0) return false;  // If there are any collisions, the placement is invalid

        // Get the positions of the 4 bottom corners of the collider
        Vector3 p = transform.position;
        Vector3 c = _collider.center;
        Vector3 e = _collider.size / 2f;
        float _bottomHeight = c.y - e.y + 0.5f;
        Vector3[] _bottomCorners = new Vector3[]
        {
            new Vector3(c.x - e.x, _bottomHeight, c.z - e.z),
            new Vector3(c.x - e.x, _bottomHeight, c.z + e.z),
            new Vector3(c.x + e.x, _bottomHeight, c.z - e.z),
            new Vector3(c.x + e.x, _bottomHeight, c.z + e.z)
        };

        // Cast a small ray beneath each corner to check for close ground
        // If at least two corners don't have valid ground, the placement is invalid
        int _invalidCornersCount = 0;
        foreach (Vector3 _corner in _bottomCorners)
        {
            if (!Physics.Raycast(
                p + _corner,
                Vector3.up * -1f,
                2f,
                Global.TERRAIN_LAYER_MASK
            ))
                _invalidCornersCount++;
        }

        return _invalidCornersCount < 3;
    }


    public override void HealthUpdate()
    {
        base.HealthUpdate();

        if (HealthPoints <= 0)
        {
            Destroy(gameObject);
        }

    }

    // Determine if the BuildingManager is active (used for base class UnitManager)
    protected override bool IsActive()
    {
        base.IsActive();
        return building.IsFixed;
    }

    public bool CanBuild(UnitData data)
    {
        gameResourceManager = FindAnyObjectByType<GameResourceManager>();
        gameResourceManager.GetTotalItemsTypeAndCount();

        int foundItems = 0;
        foreach (ItemTypeAndCount neededItemsAndCount in data.resourcesToBuild)
        {
            foreach (ItemTypeAndCount foundItemAndCount in Global.TotalItemsTypeAndCount)
            {
                if (foundItemAndCount.item == neededItemsAndCount.item && foundItemAndCount.count >= neededItemsAndCount.count)
                {
                    foundItems++;
                    break;
                }
            }
        }

        return foundItems == data.resourcesToBuild.Length;
    }

    protected override void SelectUtil()
    {
        base.SelectUtil();

        if (Global.SELECTED_BUILDINGS.Contains(this)) return;
        Global.SELECTED_BUILDINGS.Add(this);

        AddMaterial(OutilineMaterial);
    }

    // Select the unit, allowing for multiple selections with or without the Shift key
    public override void Select()
    {
        Select(false, false);
    }


    public override void Select(bool _singleClick, bool _holdingShift)
    {
        base.Select();
        // Basic case: using the selection box
        if (!_singleClick)
        {
            SelectUtil();
            return;
        }

        // Single click: check for Shift key
        if (!_holdingShift)
        {
            List<BuildingManager> selectedUnits = new List<BuildingManager>(Global.SELECTED_BUILDINGS);
            foreach (BuildingManager um in selectedUnits)

                um.Deselect();
            SelectUtil();


        }
        else
        {
            if (!Global.SELECTED_BUILDINGS.Contains(this))
                SelectUtil();
            else
                Deselect();
        }
    }

        public override void Deselect()
    {
        base.Deselect();

        if (!Global.SELECTED_BUILDINGS.Contains(this)) return;
        Global.SELECTED_BUILDINGS.Remove(this);

        RemoveMaterial("M_Outline (Instance)");

        if (buildingActionSelection != null)
        {
            buildingActionSelection.TransferPanel.SetActive(false);
        }
    }

    public override void AddMaterial(Material material)
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

    public override void RemoveMaterial(string materialName) 
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
