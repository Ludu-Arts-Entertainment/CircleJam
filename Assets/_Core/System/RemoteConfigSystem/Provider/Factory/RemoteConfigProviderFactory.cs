using System.Collections.Generic;

public enum RemoteConfigProviderEnums
{
    TestRemoteConfigProvider,
#if FirebaseRemoteConfig_Enabled
    FirebaseRemoteConfigProvider
#endif
}
public static class RemoteConfigProviderFactory
{
    private static Dictionary<RemoteConfigProviderEnums, IRemoteConfigProvider> _remoteConfigProviderDictionary = new ()
    {
        {RemoteConfigProviderEnums.TestRemoteConfigProvider, new TestRemoteConfigProvider()},
#if FirebaseRemoteConfig_Enabled
        {RemoteConfigProviderEnums.FirebaseRemoteConfigProvider, new FirebaseRemoteConfigProvider()}
#endif
    };

    public static IRemoteConfigProvider Create(RemoteConfigProviderEnums providerEnum)
    {
        return _remoteConfigProviderDictionary.TryGetValue(providerEnum, out var provider) ? provider.CreateSelf() : null;
    }
    
}
