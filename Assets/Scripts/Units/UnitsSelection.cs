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

        if (Global.SELECTED_UNITS.Count > 0)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                DeselectAllUnits();
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
                        DeselectAllUnits();
  
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
            // Check if the unit's position is within the selection bounds
            inBounds = selectionBounds.Contains(
                Camera.main.WorldToViewportPoint(unit.transform.position)
            );
            if (inBounds)
                unit.GetComponent<UnitManager>().Select();
            else
                unit.GetComponent<UnitManager>().Deselect();
        }
    }

    private void DeselectAllUnits()
    {
        // Deselect all currently selected units
        List<UnitManager> selectedUnits = new List<UnitManager>(Global.SELECTED_UNITS);
        foreach (UnitManager um in selectedUnits)
            um.Deselect();
    }
}
