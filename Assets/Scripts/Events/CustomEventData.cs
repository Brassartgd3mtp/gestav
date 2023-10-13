using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomEventData : MonoBehaviour

// This script stores the data of our events
{
    public BuildingData buildingData;

    public CustomEventData(BuildingData _buildingData)
    {
        this.buildingData = _buildingData;
    }
}

[System.Serializable]
public class CustomEvent : UnityEvent<CustomEventData> { }

