using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Flags]
public enum InventoryResourceType
{
    Copper = 1 << 0,
    Wood = 1 << 1,
    CopperBar = 1 <<2,
    CopperScrew = 1 <<3,
    Worker = 1 <<4
}
//type |= valideType

public class InventoryHolder : MonoBehaviour
{
    public InventoryResourceType validType;
    [SerializeField] private int inventorySize;
    [SerializeField] protected InventorySystem inventorySystem;

    public InventorySystem InventorySystem => inventorySystem;

    public static UnityAction<InventorySystem> OnDynamicInventoryDisplayRequested;

    protected virtual void Awake()
    {
        inventorySystem = new InventorySystem(inventorySize);
    }
}
