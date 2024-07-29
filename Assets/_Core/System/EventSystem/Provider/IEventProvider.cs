using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEventProvider
{
    IEventProvider CreateSelf();
    Dictionary<Type,List<object>> EventDictionary { get; set; }
    Dictionary<Type,List<Action>> NoArgsEventDictionary { get; set; }
    void Subscribe<T>(Action<T> action) where T : IEvent;
    void Subscribe<T>(Action action) where T : IEvent;
    void Unsubscribe<T>(Action<T> action) where T : IEvent;
    void Unsubscribe<T>(Action action) where T : IEvent;
    void Trigger<T>(T eventObject) where T : IEvent;
}
