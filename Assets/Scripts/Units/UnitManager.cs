using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider))]
public class UnitManager : MonoBehaviour
{
    [Header("Statistics")]
    public int HealthPoints;

    public Material OutilineMaterial;

    protected BoxCollider _collider;
    protected BuildingActionSelection buildingActionSelection;

    [SerializeField] protected Slider healthBar;

    private void Awake()
    {
        buildingActionSelection = GetComponentInChildren<BuildingActionSelection>();
    }


    protected virtual void FixedUpdate()
    {
        HealthUpdate();
    }
    public virtual void  HealthUpdate()
    {
        healthBar.value = HealthPoints;
    }
    protected virtual Unit Unit { get; set; }

    // Called when the mouse is clicked on the unit
    private void OnMouseDown()
    {
        bool isOverUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
        if (IsActive() && !isOverUI)
            Select(true, Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
    }

    // Check if the unit is active and can be selected
    protected virtual bool IsActive()
    {
        return true;
    }

    // Utility method for selecting the unit
    protected virtual void SelectUtil()
    {

        if (Global.SELECTED_UNITS.Contains(this)) return;
        Global.SELECTED_UNITS.Add(this);
    }

    // Select the unit, allowing for multiple selections with or without the Shift key
    public virtual void Select()
    {

    }

    public virtual void Select(bool _singleClick, bool _holdingShift)
    {
        if (!_singleClick)
        {
            SelectUtil();
            return;
        }

        // Single click: check for Shift key
        if (!_holdingShift)
        {
            List<UnitManager> selectedUnits = new List<UnitManager>(Global.SELECTED_CHARACTERS);
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
    public virtual void Deselect()
    {
        if (!Global.SELECTED_UNITS.Contains(this)) return;
        Global.SELECTED_UNITS.Remove(this);
    }


    public void Initialize(Unit _unit)
    {
        _collider = GetComponent<BoxCollider>();
        Unit = _unit;
    }

    public virtual void AddMaterial(Material material)
    {


    }

    public virtual void RemoveMaterial(string materialName)
    {


    }
}
