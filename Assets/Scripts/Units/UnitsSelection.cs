using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class UnitsSelection : MonoBehaviour
{
    
    private bool isDraggingMouseBox = false;

    private Vector3 dragStartPosition;
    [SerializeField] private float dragThreshold;

    Ray ray;
    RaycastHit raycastHit;

    private float mouseButtonDownTime;

    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
                isDraggingMouseBox = true;
                dragStartPosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDraggingMouseBox = false;
        }


        if (isDraggingMouseBox && Vector3.Distance(dragStartPosition, Input.mousePosition) > dragThreshold)
        {
            SelectUnitsInDraggingBox();
        }

        if (Global.SELECTED_CHARACTERS.Count > 0)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                DeselectAllUnits();
            if (Input.GetMouseButtonDown(0))
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
              // bool isNotOverUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
                if (Physics.Raycast(
                    ray,
                    out raycastHit,
                    1000f
                ))
                {
                    if (raycastHit.collider.gameObject.TryGetComponent(out EnemyManager em))
                    {
                        foreach (UnitManager um in Global.SELECTED_CHARACTERS)
                            if (um is HeroManager)
                            {
                                HeroManager hm = um as HeroManager;
                                hm.CurrentTarget = em;
                            }
                    }
                    else if (raycastHit.transform.tag == "Terrain" /*&& !isNotOverUI */)
                        DeselectAllUnits();
                }
            }
        }

        if (Global.SELECTED_BUILDINGS.Count > 0)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                DeselectAllBuildings();
            if (Input.GetMouseButtonDown(0))
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                bool isNotOverUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
                if (Physics.Raycast(
                    ray,
                    out raycastHit,
                    1000f
                ))
                {
                    if (raycastHit.transform.tag == "Terrain" && !isNotOverUI)
                        DeselectAllBuildings();

                }
            }
        }
    }

    void OnGUI()
    {
        if (isDraggingMouseBox && Vector3.Distance(dragStartPosition, Input.mousePosition) > dragThreshold)
        {
            // Create a rectangle from both mouse positions
            var rect = Utils.GetScreenRect(dragStartPosition, Input.mousePosition);
            Utils.DrawScreenRect(rect, new Color(0.5f, 1f, 0.4f, 0.2f));
            Utils.DrawScreenRectBorder(rect, 1, new Color(0.5f, 1f, 0.4f));
        }
    }

    private void SelectUnitsInDraggingBox()
    {
        // Calculate the selection bounds based on the dragged box
        Bounds selectionBounds = Utils.GetViewportBounds(
            Camera.main,
            dragStartPosition,
            Input.mousePosition
        );
        GameObject[] selectableUnits = GameObject.FindGameObjectsWithTag("Unit");
        bool inBounds;
        foreach (GameObject unit in selectableUnits)
        {
            if (unit.GetComponent<BuildingManager>() == null)
            {
                // Check if the unit's position is within the selection bounds
                inBounds = selectionBounds.Contains(
                    Camera.main.WorldToViewportPoint(unit.transform.position)
                );
                if (inBounds)
                    unit.GetComponent<UnitManager>().Select();
                else
                    unit.GetComponent<UnitManager>().Deselect();
            }
            else
            {
                // Check if the unit's position is within the selection bounds
                inBounds = selectionBounds.Contains(
                    Camera.main.WorldToViewportPoint(unit.transform.position)
                );
                if (inBounds)
                    unit.GetComponent<BuildingManager>().Select();
                else
                    unit.GetComponent<BuildingManager>().Deselect();
            }

        }
    }

    private void DeselectAllUnits()
    {
        // Deselect all currently selected units
        List<CharacterManager> selectedUnits = new List<CharacterManager>(Global.SELECTED_CHARACTERS);
        foreach (CharacterManager um in selectedUnits)
            um.Deselect();
    }
    private void DeselectAllBuildings()
    {
        // Deselect all currently selected units
        List<BuildingManager> selectedUnits = new List<BuildingManager>(Global.SELECTED_BUILDINGS);
        foreach (BuildingManager um in selectedUnits)
            um.Deselect();
    }
}
