using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BuildingInventory : UnitInventory, IInteractable
{
    protected override void Awake()
    {
        base.Awake();
        Global.BUILDINGS.Add(this);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        Global.BUILDINGS.Remove(this);
    }
}
