using System;
using System.Collections.Generic;

public class TestRemoteConfigProvider : IRemoteConfigProvider
{
    private readonly Dictionary<Type, object> _remoteConfigDictionary = new DefaultRemoteData().RemoteConfigDictionary;
    public IRemoteConfigProvider CreateSelf()
    {
        return new TestRemoteConfigProvider();
    }

    public void Initialize(Action onReady)
    {
        onReady?.Invoke();
    }

    public T GetObject<T>(T defaultValue) where T : IConfig
    {
        if (_remoteConfigDictionary.TryGetValue(typeof(T), out var value))
        {
            return (T)value;
        }
        return defaultValue ?? (T)Activator.CreateInstance(typeof(T));
    }
}
