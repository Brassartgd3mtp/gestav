using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingPlacer : MonoBehaviour
{
    private Building placedBuilding = null; // The building that is currently being placed

    private Ray ray; // A ray used for raycasting
    private RaycastHit raycastHit; // Stores information about the object hit by the ray
    private Vector3 lastPlacementPosition; // The last known position where the building was placed

    void Update()
    {
        if (placedBuilding != null)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CancelPlacedBuilding(); // Cancel building placement
                return;
            }

            // Perform a raycast from the mouse pointer
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit, 1000f, Global.TERRAIN_LAYER_MASK))
            {
                // Set the position of the building to the cursor's position
                placedBuilding.SetPosition(new Vector3(raycastHit.point.x, 0 , raycastHit.point.z));
                if (lastPlacementPosition != raycastHit.point)
                {
                    placedBuilding.CheckValidPlacement();
                }
                lastPlacementPosition = raycastHit.point;
            }

            if (placedBuilding.HasValidPlacement && Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                PlaceBuilding(); // Place the building if the placement is valid and the mouse button is released
            }
        }
    }

    // Select a building to be placed by specifying its data index
    public void SelectPlacedBuilding(int _buildingDataIndex)
    {
        PreparePlacedBuilding(_buildingDataIndex);
    }

    // Prepare a building to be placed
    void PreparePlacedBuilding(int _buildingDataIndex)
    {
        // Destroy the previous "phantom" building if there is one
        if (placedBuilding != null && !placedBuilding.IsFixed)
        {
            Destroy(placedBuilding.Transform.gameObject);
        }

        // Create a new "phantom" building based on the specified building data
        Building _building = new Building(Global.BUILDING_DATA[_buildingDataIndex]);

        // Initialize the building's manager and link the data
        _building.Transform.GetComponent<BuildingManager>().Initialize(_building);

        placedBuilding = _building; // Set the current "phantom" building
        lastPlacementPosition = Vector3.zero; // Reset the last placement position
    }

    // Cancel the placement of the current "phantom" building
    void CancelPlacedBuilding()
    {
        // Destroy the "phantom" building
        Destroy(placedBuilding.Transform.gameObject);
        placedBuilding = null; // Set the placedBuilding to null
    }

    // Place the current "phantom" building
    void PlaceBuilding()
    {
        placedBuilding.Place(); // Set the placement state to "fixed"

        // Continue building the same type of building

        PreparePlacedBuilding(placedBuilding.DataIndex);
    }
}
