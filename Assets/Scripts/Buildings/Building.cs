using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public enum BuildingPlacement
{
    VALID,
    FIXED,
    INVALID
};

public class Building //This script handles the instances of the buildings
{
    public BuildingData data;
    private Transform transform;
    private BuildingPlacement placement;
    private int currentHealth;
    private List<Material> _materials;

    private BuildingManager buildingManager;

    public Building(BuildingData _data) //create a new Building instance using a BuildingData reference so it has all the required metadata
    {
        data = _data;
        currentHealth = _data.HP; //set the current health of the building to be the one set in the data script

        Debug.Log($"data.Code: {data.Code}");
        GameObject g = GameObject.Instantiate(Resources.Load($"Prefabs/Buildings/{data.Code}")) as GameObject; // instantiate the gameobject from the prefab located in our resources folder
        transform = g.transform;

        // set building mode as "valid" placement
        placement = BuildingPlacement.VALID;

        _materials = new List<Material>();
        foreach (Material material in transform.Find("Mesh").GetComponent<Renderer>().materials)
        {
            _materials.Add(new Material(material));
        }

        // (set the materials to match the "valid" initial state)
        buildingManager = g.GetComponent<BuildingManager>();
        placement = BuildingPlacement.VALID;
        SetMaterials();
    }


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
            return;
        }
        transform.Find("Mesh").GetComponent<Renderer>().materials = materials.ToArray();
    }




    public void Place()
    {
        // set placement state
        placement = BuildingPlacement.FIXED;
        // change building materials
        SetMaterials();
        // remove "is trigger" flag from box collider to allow
        // for collisions with units
        transform.GetComponent<BoxCollider>().isTrigger = false;
    }


    public void CheckValidPlacement()
    {
        if (placement == BuildingPlacement.FIXED) return;
        placement = buildingManager.CheckPlacement()
            ? BuildingPlacement.VALID
            : BuildingPlacement.INVALID;
    }

    public void SetPosition(Vector3 _position) // set the position of the object in the scene
    {
        transform.position = _position;
    }

    public string Code { get => data.Code; }
    public Transform Transform { get => transform; }
    public int HP { get => currentHealth; set => currentHealth = value; } // setter will allow a quick way to update the HP form outside the class
    public int MaxHP { get => data.HP; }
    public int DataIndex // gives us the index of the abstract building type data instance in the global list
    {
        get
        {
            for (int i = 0; i < Global.BUILDING_DATA.Length; i++)
            {
                if (Global.BUILDING_DATA[i].Code == data.Code)
                {
                    return i;
                }
            }
            return -1;
        }
    }
    public bool IsFixed { get => placement == BuildingPlacement.FIXED; }
    public bool HasValidPlacement { get => placement == BuildingPlacement.VALID; }
}

