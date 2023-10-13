using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public GameObject selectionCircle;

    public void Select()
    {
        if (Global.SELECTED_UNITS.Contains(this)) return;
        Global.SELECTED_UNITS.Add(this);
        selectionCircle.SetActive(true);
    }

    public void Deselect()
    {
        if (!Global.SELECTED_UNITS.Contains(this)) return;
        Global.SELECTED_UNITS.Remove(this);
        selectionCircle.SetActive(false);
    }
}
