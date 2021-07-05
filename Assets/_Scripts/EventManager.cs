using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private Dictionary<string, Action<EventParameter>> events;

    private static EventManager eventManager;

    public static EventManager Instance
    {
        get 
        {
            return eventManager;
        }
    }

    private void Awake()
    {
        events = new Dictionary<string, Action<EventParameter>>();
        if (!eventManager) eventManager = this;
        //Debug.Log(Instance);

    }

    public static void StartListening(string eventName, Action<EventParameter> listener)
    {
        //Debug.Log(Instance);
        if (Instance.events.ContainsKey(eventName))
        {
            Instance.events[eventName] += listener;
        }
        else
        {
            Instance.events.Add(eventName, listener);
        }
    }

    public static void StopListening(string eventName, Action<EventParameter> listener)
    {
        if (Instance.events.ContainsKey(eventName))
        {
            Instance.events[eventName] -= listener;
        }
    }

    public static void TriggerAction(string eventName, EventParameter eventParam)
    {
        Action<EventParameter> newEvent = null;
        if (Instance.events.TryGetValue(eventName, out newEvent))
        {
            newEvent.Invoke(eventParam);
        }
    }
}

public struct EventParameter
{
    public float floatVar;
    public float floatVar2;
    public float floatVar3;
    public int intVar;
    public int intVar2;
    public bool boolVar;
    public string stringVar;
}

//public struct SoundEventParameter : EventParameter
//{

//}