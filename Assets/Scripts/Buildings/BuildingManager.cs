using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider))]
public class BuildingManager : UnitManager
{
    private BoxCollider _collider;

    private Building building = null;
    private int nCollisions = 0;

    public void Initialize(Building _building)
    {
        _collider = GetComponent<BoxCollider>();
        building = _building;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Terrain") return;
        nCollisions++;
        CheckPlacement();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Terrain") return;
        nCollisions--;
        CheckPlacement();
    }

    public bool CheckPlacement()
    {
        if (building == null) return false;
        if (building.IsFixed) return false;
        bool _validPlacement = HasValidPlacement();
        if (!_validPlacement)
        {
            building.SetMaterials(BuildingPlacement.INVALID);
        }
        else
        {
            building.SetMaterials(BuildingPlacement.VALID);
        }
        return _validPlacement;
    }

    public bool HasValidPlacement()
    {
        if (nCollisions > 0) return false;

        // get 4 bottom corner positions
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
        // cast a small ray beneath the corner to check for a close ground
        // (if at least two are not valid, then placement is invalid)
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
}
