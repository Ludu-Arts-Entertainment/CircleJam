using System;
using System.Collections.Generic;
using UnityEngine;

public class BasicEventProvider : IEventProvider
{
    public Dictionary<Type, List<object>> EventDictionary { get; set; }
    public Dictionary<Type, List<Action>> NoArgsEventDictionary { get; set; }

    public BasicEventProvider()
    {
        EventDictionary = new ();
        NoArgsEventDictionary = new ();
    }
    public IEventProvider CreateSelf()
    {
        return new BasicEventProvider();
    }

    public void Subscribe<T>(Action<T> action) where T : IEvent
    {
        var type = typeof(T);
        if (!EventDictionary.ContainsKey(type)) EventDictionary[type] = new List<object>();
        EventDictionary[type].Add(action);
        //Debug.Log("Subscribed to " + type);
    }

    public void Subscribe<T>(Action action) where T : IEvent
    {
        var type = typeof(T);
        if (!NoArgsEventDictionary.ContainsKey(type)) NoArgsEventDictionary[type] = new List<Action>();
        NoArgsEventDictionary[type].Add(action);
        //Debug.Log("Subscribed to " + type);
    }

    public void Unsubscribe<T>(Action<T> action) where T : IEvent
    {
        //Debug.Log("Unsubscribed from " + typeof(T));
        var type = typeof(T);
        if (EventDictionary.ContainsKey(type))
        {
            var list = EventDictionary[type];
            if (list.Contains(action)) list.Remove(action);

            if (list.Count == 0) EventDictionary.Remove(type);
        }
        else
        {
            Debug.LogWarning("EventBus: Unsubscribe failed, no event of type " + type);
        }
    }

    public void Unsubscribe<T>(Action action) where T : IEvent
    {
        //Debug.Log("Unsubscribed from " + typeof(T));
        var type = typeof(T);
        if (NoArgsEventDictionary.ContainsKey(type))
        {
            var list = NoArgsEventDictionary[type];
            if (list.Contains(action)) list.Remove(action);

            if (list.Count == 0) NoArgsEventDictionary.Remove(type);
        }
        else
        {
            Debug.LogWarning("EventBus: Unsubscribe failed, no event of type " + type);
        }
    }
    public void Trigger<T>(T payload) where T : IEvent
    {
        var type = typeof(T);
        if (EventDictionary.ContainsKey(type))
        {
            object[] registereds = new object[EventDictionary[type].Count];
            for (int i = 0; i < registereds.Length; i++)
            {
                registereds[i] = EventDictionary[type][i];
            }

            for (int index = 0; index < registereds.Length; index++)
            {
                Action<T> action = (Action<T>)registereds[index];

                try
                {
                    action?.Invoke(payload);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }

        if (NoArgsEventDictionary.TryGetValue(type, out var value))
        {
            var actions = new List<Action>(value);
            for (int index = 0; index < actions.Count; index++)
            {
                Action action = actions[index];

                try
                {
                    action?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
//        Debug.Log("Triggered " + type + " event");
    }
}
