using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : IManager
{
    private IEventProvider _eventProvider;
    public IManager CreateSelf()
    {
        return new EventManager();
    }
    public void Initialize(GameInstaller gameInstaller, Action onReady)
    {
        _eventProvider = EventProviderFactory.Create(gameInstaller.Customizer.EventProvider);
        onReady.Invoke();
    }
    public bool IsReady()
    {
        return _eventProvider != null;
    }
    public void Subscribe<T>(Action<T> action) where T : IEvent
    {
        _eventProvider.Subscribe<T>(action);
    }
    public void Subscribe<T>(Action action) where T : IEvent
    {
        _eventProvider.Subscribe<T>(action);
    }
    public void Unsubscribe<T>(Action<T> action) where T : IEvent
    {
        _eventProvider.Unsubscribe(action);
    }
    public void Unsubscribe<T>(Action action) where T : IEvent
    {
        _eventProvider.Unsubscribe<T>(action);
    }
    public void Trigger<T>(T eventObject) where T : IEvent
    {
        _eventProvider.Trigger(eventObject);
    }
}
