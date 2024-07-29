using System.Collections.Generic;
public enum EventProviderEnums
{
    BasicEventProvider,
}

public static class EventProviderFactory
{
    private static Dictionary<EventProviderEnums,IEventProvider> _eventProviderDictionary = new ()
    {
        {EventProviderEnums.BasicEventProvider, new BasicEventProvider()},
    };
    
    public static IEventProvider Create(EventProviderEnums providerEnum)
    {
        return _eventProviderDictionary.TryGetValue(providerEnum, out var provider) ? provider.CreateSelf() : null;
    }
}

