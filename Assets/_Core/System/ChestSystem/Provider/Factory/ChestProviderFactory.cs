
using System.Collections.Generic;

public enum ChestProviderEnums
{
    BasicChestProvider
}
public static class ChestProviderFactory
{
    private static Dictionary<ChestProviderEnums, IChestProvider> _chestProviderDictionary = new ()
    {
        {ChestProviderEnums.BasicChestProvider , new BasicChestProvider()},
    };
    
    public static IChestProvider Create(ChestProviderEnums providerEnum)
    {
        return _chestProviderDictionary.TryGetValue(providerEnum, out var provider) ? provider.CreateSelf() : null;
    }
}
