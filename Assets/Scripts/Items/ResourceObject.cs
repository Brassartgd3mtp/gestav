using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Resource", menuName = "Inventory System/Items/GameResource") ]

public class ResourceObject : ItemObject
{

    public void Awake()
    {
        type = ItemType.GameResource;
    }

}
