using System;
using System.Collections.Generic;
using UnityEngine;

public class EventSystem
{
    private Dictionary<string, List<Action<object>>> eventListeners;

    public EventSystem()
    {
        eventListeners = new Dictionary<string, List<Action<object>>>();
    }

    public void Subscribe(string eventName, Action<object> listener)
    {
        if (!eventListeners.ContainsKey(eventName))
        {
            eventListeners[eventName] = new List<Action<object>>();
        }
        eventListeners[eventName].Add(listener);
    }

    public void Unsubscribe(string eventName, Action<object> listener)
    {
        if (eventListeners.ContainsKey(eventName))
        {
            eventListeners[eventName].Remove(listener);
        }
    }

    public void Publish(string eventName, object data = null)
    {
        if (eventListeners.ContainsKey(eventName))
        {
            foreach (var listener in eventListeners[eventName])
            {
                listener?.Invoke(data);
            }
        }
    }
}