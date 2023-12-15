using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;

// Enumeration to represent the building placement state
public enum BuildingPlacement
{
    VALID,   // Valid placement
    FIXED,   // Placement fixed
    INVALID  // Invalid placement
};

// Class representing a building in the game
public class Building : Unit
{
    private BuildingPlacement placement;   // Building placement state
    private List<Material> _materials;    // List of materials for rendering

    private BuildingManager buildingManager;  // Building manager
    private BuildingStockageUI buildingStockageUI;

    // Constructor for the Building class
    public Building(BuildingData _data) : base(_data)
    {
        buildingManager = Transform.GetComponent<BuildingManager>();

        // Copy the initial rendering materials to the _materials list
        _materials = new List<Material>();
        foreach (Material material in transform.GetComponentInChildren<MeshRenderer>().materials)
        {
            _materials.Add(new Material(material));
        }


        placement = BuildingPlacement.VALID;
        SetMaterials();
    }

    // Method to set the building's materials
    public void SetMaterials() { SetMaterials(placement); }
    public void SetMaterials(BuildingPlacement _placement)
    {
        List<Material> materials;

        if (_placement == BuildingPlacement.VALID)
        {
            Material refMaterial = Resources.Load("Materials/Valid") as Material;
            materials = new List<Material>();
            for (int i = 0; i < _materials.Count; i++)
            {
                materials.Add(refMaterial);
            }
        }
        else if (_placement == BuildingPlacement.INVALID)
        {
            Material refMaterial = Resources.Load("Materials/Invalid") as Material;
            materials = new List<Material>();
            for (int i = 0; i < _materials.Count; i++)
            {
                materials.Add(refMaterial);
            }
        }
        else if (_placement == BuildingPlacement.FIXED)
        {
            materials = _materials;
        }
        else
        {
            return;  // If the placement state is not recognized, do nothing
        }

        // Apply the materials to the building's rendering
        transform.GetComponentInChildren<MeshRenderer>().materials = materials.ToArray();
    }

    // Method to place the building
    public override void Place()
    {
    //    if(buildingManager.CanBuild(data))
    //    {
            base.Place();
            // Set the placement state to "fixed"
            placement = BuildingPlacement.FIXED;

            // Change the building's materials
            SetMaterials();

            //rebuild the navmesh
            Global.RebuildNavMesh();
     //   }

    }



    // Method to check if the building's placement is valid
    public void CheckValidPlacement()
    {
        if (placement == BuildingPlacement.FIXED) return;

        // Check the placement with the building manager
        placement = buildingManager.CheckPlacement()
            ? BuildingPlacement.VALID
            : BuildingPlacement.INVALID;
    }

    // Property to get the index of the building's data in the global list
    public int DataIndex
    {
        get
        {
            for (int i = 0; i < Global.BUILDING_DATA.Length; i++)
            {
                if (Global.BUILDING_DATA[i].code == data.code)
                {
                    return i;
                }
            }
            return -1;  // Return -1 if the data is not found
        }
    }

    // Property to check if the building is fixed in place
    public bool IsFixed { get => placement == BuildingPlacement.FIXED; }

    // Property to check if the building has a valid placement
    public bool HasValidPlacement { get => placement == BuildingPlacement.VALID; }

}
