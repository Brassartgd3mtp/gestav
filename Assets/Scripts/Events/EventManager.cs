using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    private Dictionary<string, UnityEvent> events; // Dictionary to store events with no custom data
    private static EventManager eventManager; // Singleton instance of the EventManager
    private Dictionary<string, CustomEvent> typedEvents; // Dictionary to store events with custom data

    // Singleton pattern to get an instance of EventManager
    public static EventManager instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                if (!eventManager)
                    Debug.LogError("There needs to be one active EventManager script on a GameObject in your scene.");
                else
                    eventManager.Init();
            }

            return eventManager;
        }
    }

    // Initialize event dictionaries
    void Init()
    {
        if (events == null)
        {
            events = new Dictionary<string, UnityEvent>();
            typedEvents = new Dictionary<string, CustomEvent>();
        }
    }

    // Add a listener to an event without custom data
    public static void AddListener(string _eventName, UnityAction _listener)
    {
        UnityEvent _evt = null;
        if (instance.events.TryGetValue(_eventName, out _evt))
        {
            _evt.AddListener(_listener);
        }
        else
        {
            _evt = new UnityEvent();
            _evt.AddListener(_listener);
            instance.events.Add(_eventName, _evt);
        }
    }

    // Remove a listener from an event without custom data
    public static void RemoveListener(string _eventName, UnityAction _listener)
    {
        if (eventManager == null) return;
        UnityEvent _evt = null;
        if (instance.events.TryGetValue(_eventName, out _evt))
            _evt.RemoveListener(_listener);
    }

    // Trigger an event without custom data
    public static void TriggerEvent(string _eventName)
    {
        UnityEvent _evt = null;
        if (instance.events.TryGetValue(_eventName, out _evt))
            _evt.Invoke();
    }

    // Add a listener to an event with custom data
    public static void AddTypedListener(string _eventName, UnityAction<CustomEventData> _listener)
    {
        CustomEvent _evt = null;
        if (instance.typedEvents.TryGetValue(_eventName, out _evt))
        {
            _evt.AddListener(_listener);
        }
        else
        {
            _evt = new CustomEvent();
            _evt.AddListener(_listener);
            instance.typedEvents.Add(_eventName, _evt);
        }
    }

    // Remove a listener from an event with custom data
    public static void RemoveTypedListener(string _eventName, UnityAction<CustomEventData> _listener)
    {
        if (eventManager == null) return;
        CustomEvent _evt = null;
        if (instance.typedEvents.TryGetValue(_eventName, out _evt))
            _evt.RemoveListener(_listener);
    }

    // Trigger an event with custom data
    public static void TriggerTypedEvent(string _eventName, CustomEventData _data)
    {
        CustomEvent _evt = null;
        if (instance.typedEvents.TryGetValue(_eventName, out _evt))
            _evt.Invoke(_data);
    }
}
