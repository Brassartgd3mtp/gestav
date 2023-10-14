using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// This script stores the data associated with our events
public class CustomEventData : MonoBehaviour
{
    public BuildingData buildingData; // Data associated with the event

    // Constructor for CustomEventData that initializes it with BuildingData
    public CustomEventData(BuildingData _buildingData)
    {
        this.buildingData = _buildingData;
    }
}

// A custom event class that extends UnityEvent with CustomEventData as its parameter
[System.Serializable]
public class CustomEvent : UnityEvent<CustomEventData> { }
