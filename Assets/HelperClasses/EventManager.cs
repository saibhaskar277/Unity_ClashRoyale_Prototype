using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager
{
    private static readonly Dictionary<Type, Delegate> eventTable = new();

    // ✅ Subscribe
    public static void AddListner<T>(Action<T> listener) where T : IGameEvent
    {
        Type type = typeof(T);

        if (eventTable.TryGetValue(type, out var existing))
        {
            eventTable[type] = Delegate.Combine(existing, listener);
        }
        else
        {
            eventTable[type] = listener;
        }
    }

    // ✅ Unsubscribe
    public static void RemoveListner<T>(Action<T> listener) where T : IGameEvent
    {
        Type type = typeof(T);

        if (!eventTable.TryGetValue(type, out var existing))
        {
            Debug.LogWarning($"Trying to remove listener for {type.Name}, but none found.");
            return;
        }

        var newDelegate = Delegate.Remove(existing, listener);

        if (newDelegate == null)
        {
            eventTable.Remove(type);
        }
        else
        {
            eventTable[type] = newDelegate;
        }
    }

    // ✅ Raise Event
    public static void RaiseEvent<T>(T gameEvent) where T : IGameEvent
    {
        Type type = typeof(T);

        if (eventTable.TryGetValue(type, out var existing))
        {
            if (existing is Action<T> callback)
            {
                callback.Invoke(gameEvent);
            }
            else
            {
                Debug.LogError($"Delegate type mismatch for event: {type.Name}");
            }
        }
        else
        {
            // Optional (comment if too noisy)
            // Debug.LogWarning($"No listeners for event: {type.Name}");
        }
    }

    // 🔥 Clear all events (VERY IMPORTANT for scene reloads)
    public static void ClearAll()
    {
        eventTable.Clear();
    }
}


public interface IGameEvent { }