using System;
using System.Collections.Generic;

public enum GameEventType
{
    PlayerMoved,
    PlatformMidpointReached,
    PlayerHitFireGround,
    GameOver,
    GameStarted
}

public class EventSystem
{
    private Dictionary<GameEventType, List<Action<object>>> eventListeners;

    public EventSystem()
    {
        eventListeners = new Dictionary<GameEventType, List<Action<object>>>();
    }

    public void Subscribe(GameEventType eventType, Action<object> listener)
    {
        if (!eventListeners.ContainsKey(eventType))
        {
            eventListeners[eventType] = new List<Action<object>>();
        }
        eventListeners[eventType].Add(listener);
    }

    public void Unsubscribe(GameEventType eventType, Action<object> listener)
    {
        if (eventListeners.ContainsKey(eventType))
        {
            eventListeners[eventType].Remove(listener);
        }
    }

    public void Publish(GameEventType eventType, object data = null)
    {
        if (eventListeners.ContainsKey(eventType))
        {
            foreach (var listener in eventListeners[eventType])
            {
                listener?.Invoke(data);
            }
        }
    }
}