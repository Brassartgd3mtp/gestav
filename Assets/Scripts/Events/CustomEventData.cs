using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// This script stores the data associated with our events
public class CustomEventData : MonoBehaviour
{
    public UnitData unitData;

    public CustomEventData(UnitData unitData)
    {
        this.unitData = unitData;
    }
}

// A custom event class that extends UnityEvent with CustomEventData as its parameter
[System.Serializable]
public class CustomEvent : UnityEvent<CustomEventData> { }
