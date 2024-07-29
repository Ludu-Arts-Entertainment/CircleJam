using System.Collections.Generic;

public enum PoolProviderEnums
{
    BasicPoolProvider,
}
public static class PoolProviderFactory
{
    private static Dictionary<PoolProviderEnums,IPoolProvider> _poolProviderDictionary = new ()
    {
        {PoolProviderEnums.BasicPoolProvider, new BasicPoolProvider()},
    };
    
    public static IPoolProvider Create(PoolProviderEnums providerEnum)
    {
        return _poolProviderDictionary.TryGetValue(providerEnum, out var provider) ? provider.CreateSelf() : null;
    }
}
