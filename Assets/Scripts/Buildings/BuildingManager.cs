using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Required component for the script, automatically adds a BoxCollider if it's not present
[RequireComponent(typeof(BoxCollider))]
public class BuildingManager : UnitManager
{
    private BoxCollider _collider;  // Reference to the BoxCollider component

    private Building building = null;  // Reference to the building managed by this script
    private int nCollisions = 0;  // Counter for collision events

    // Initialize the BuildingManager with a specific building
    public void Initialize(Building _building)
    {
        _collider = GetComponent<BoxCollider>();  // Get the BoxCollider component
        building = _building;  // Set the managed building
    }

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

    // Determine if the BuildingManager is active (used for base class UnitManager)
    protected override bool IsActive()
    {
        return building.IsFixed;
    }
}
