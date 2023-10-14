using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public GameObject selectionCircle; // Reference to the selection circle object

    // Called when the mouse is clicked on the unit
    private void OnMouseDown()
    {
        if (IsActive())
            Select(true, Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
    }

    // Check if the unit is active and can be selected
    protected virtual bool IsActive()
    {
        return true;
    }

    // Utility method for selecting the unit
    private void SelectUtil()
    {
        if (Global.SELECTED_UNITS.Contains(this)) return;
        Global.SELECTED_UNITS.Add(this);
        selectionCircle.SetActive(true);
    }

    // Select the unit, allowing for multiple selections with or without the Shift key
    public void Select()
    {
        Select(false, false);
    }

    public void Select(bool _singleClick, bool _holdingShift)
    {
        // Basic case: using the selection box
        if (!_singleClick)
        {
            SelectUtil();
            return;
        }

        // Single click: check for Shift key
        if (!_holdingShift)
        {
            List<UnitManager> selectedUnits = new List<UnitManager>(Global.SELECTED_UNITS);
            foreach (UnitManager um in selectedUnits)
                um.Deselect();
            SelectUtil();
        }
        else
        {
            if (!Global.SELECTED_UNITS.Contains(this))
                SelectUtil();
            else
                Deselect();
        }
    }

    // Deselect the unit
    public void Deselect()
    {
        if (!Global.SELECTED_UNITS.Contains(this)) return;
        Global.SELECTED_UNITS.Remove(this);
        selectionCircle.SetActive(false);
    }
}
