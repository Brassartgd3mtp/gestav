using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingPlacer : MonoBehaviour
{
    private Building placedBuilding = null;

    private Ray ray;
    private RaycastHit raycastHit;
    private Vector3 lastPlacementPosition;



    void Update()
    {
        if (placedBuilding != null)
        {
            if (Input.GetMouseButtonDown(1))
            {
                CancelPlacedBuilding();
                return;
            }

            //do the raycast
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast( ray, out raycastHit, 1000f, Global.TERRAIN_LAYER_MASK))
            {
                placedBuilding.SetPosition(raycastHit.point); //set the position of the building to build to be the position of the cursor
                if (lastPlacementPosition != raycastHit.point)
                {
                    placedBuilding.CheckValidPlacement();
                }
                lastPlacementPosition = raycastHit.point;
            }

            if (placedBuilding.HasValidPlacement && Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                PlaceBuilding(); 
            }

        }

    }

    public void SelectPlacedBuilding(int _buildingDataIndex) //pass a building type index to to have a new "phantom" building
    {
        PreparePlacedBuilding(_buildingDataIndex);
    }

    void PreparePlacedBuilding(int _buildingDataIndex)
    {

        // destroy the previous "phantom" if there is one
        if (placedBuilding != null && !placedBuilding.IsFixed)
        {
            Destroy(placedBuilding.Transform.gameObject);
        }
        Building _building = new Building(
            Global.BUILDING_DATA[_buildingDataIndex]
        );
      
        _building.Transform.GetComponent<BuildingManager>().Initialize(_building);   // link the data into the manager

        placedBuilding = _building;
        lastPlacementPosition = Vector3.zero;
    }

    void CancelPlacedBuilding() //Cancel
    {
        // destroy the "phantom" building
        Destroy(placedBuilding.Transform.gameObject);
        placedBuilding = null;
    }

    void PlaceBuilding()
    {
        placedBuilding.Place();
        // keep on building the same building type
        PreparePlacedBuilding(placedBuilding.DataIndex);
    }

}
