using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class TypedEvent : UnityEvent<object> { }

public class EventManager : MonoBehaviour
{

    private Dictionary<string, UnityEvent> events;
    private Dictionary<string, TypedEvent> typedEvents;
    private static EventManager eventManager;

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

    void Init()
    {
        if (events == null)
        {
            events = new Dictionary<string, UnityEvent>();
            typedEvents = new Dictionary<string, TypedEvent>();
        }
    }

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
    public static void AddListener(string _eventName, UnityAction<object> _listener)
    {
        TypedEvent _evt = null;
        if (instance.typedEvents.TryGetValue(_eventName, out _evt))
        {
            _evt.AddListener(_listener);
        }
        else
        {
            _evt = new TypedEvent();
            _evt.AddListener(_listener);
            instance.typedEvents.Add(_eventName, _evt);
        }
    }

    public static void RemoveListener(string _eventName, UnityAction _listener)
    {
        if (eventManager == null) return;
        UnityEvent _evt = null;
        if (instance.events.TryGetValue(_eventName, out _evt))
            _evt.RemoveListener(_listener);
    }
    public static void RemoveListener(string _eventName, UnityAction<object> _listener)
    {
        if (eventManager == null) return;
        TypedEvent _evt = null;
        if (instance.typedEvents.TryGetValue(_eventName, out _evt))
            _evt.RemoveListener(_listener);
    }

    public static void TriggerEvent(string _eventName)
    {
        UnityEvent _evt = null;
        if (instance.events.TryGetValue(_eventName, out _evt))
            _evt.Invoke();
    }
    public static void TriggerEvent(string _eventName, object _data)
    {
        TypedEvent _evt = null;
        if (instance.typedEvents.TryGetValue(_eventName, out _evt))
            _evt.Invoke(_data);
    }
}