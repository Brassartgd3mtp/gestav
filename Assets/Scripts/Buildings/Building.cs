using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// Enumeration to represent the building placement state
public enum BuildingPlacement
{
    VALID,   // Valid placement
    FIXED,   // Placement fixed
    INVALID  // Invalid placement
};

// Class representing a building in the game
public class Building
{
    public BuildingData data;              // Building data
    private Transform transform;           // Reference to the building's Transform component
    private BuildingPlacement placement;   // Building placement state
    private int currentHealth;             // Current health points of the building
    private List<Material> _materials;    // List of materials for rendering

    private BuildingManager buildingManager;  // Building manager

    // Constructor for the Building class
    public Building(BuildingData _data)
    {
        data = _data;
        currentHealth = _data.healthpoints;

        // Instantiate a GameObject based on the building's code from a prefab
        GameObject g = GameObject.Instantiate(data.prefab) as GameObject;
        transform = g.transform;

        // Set the materials to match the "valid" initial state
        buildingManager = g.GetComponent<BuildingManager>();

        // Copy the initial rendering materials to the _materials list
        _materials = new List<Material>();
        foreach (Material material in transform.Find("Mesh").GetComponent<Renderer>().materials)
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
        transform.Find("Mesh").GetComponent<Renderer>().materials = materials.ToArray();
    }

    // Method to place the building
    public void Place()
    {
        // Set the placement state to "fixed"
        placement = BuildingPlacement.FIXED;

        // Change the building's materials
        SetMaterials();

        // Remove the "isTrigger" flag from the collider to allow collisions with units
        transform.GetComponent<BoxCollider>().isTrigger = false;
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

    // Method to set the position of the building in the scene
    public void SetPosition(Vector3 _position)
    {
        transform.position = _position;
    }

    // Property to get the building's code
    public string Code { get => data.code; }

    // Property to get the building's Transform
    public Transform Transform { get => transform; }

    // Property to get and set the current health points of the building
    public int HP { get => currentHealth; set => currentHealth = value; }

    // Property to get the maximum health points of the building
    public int MaxHP { get => data.healthpoints; }

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
