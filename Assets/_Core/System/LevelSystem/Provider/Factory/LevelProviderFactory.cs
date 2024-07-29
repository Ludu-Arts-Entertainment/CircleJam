using System.Collections.Generic;
public enum LevelProviderEnums
{
    PrefabBaseLevelProvider,
}
public static class LevelProviderFactory
{
    private static Dictionary<LevelProviderEnums,ILevelProvider> _levelProviderDictionary = new ()
    {
        {LevelProviderEnums.PrefabBaseLevelProvider, new PrefabBaseLevelProvider()},
    };
    
    public static ILevelProvider Create(LevelProviderEnums providerEnum)
    {
        return _levelProviderDictionary.TryGetValue(providerEnum, out var provider) ? provider.CreateSelf() : null;
    }
}
