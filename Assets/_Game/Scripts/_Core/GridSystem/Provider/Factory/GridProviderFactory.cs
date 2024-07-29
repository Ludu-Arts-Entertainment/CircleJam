using System.Collections.Generic;

public enum GridProviderEnums
{
    CircleJamGridProvider,
}

public static class GridProviderFactory 
{
    private static Dictionary<GridProviderEnums, IGridProvider> _gridProviderDictionary = new ()
    {
        {GridProviderEnums.CircleJamGridProvider, new CircleJamGridProvider()}
    };
    
    public static IGridProvider Create(GridProviderEnums providerEnum)
    {
        return _gridProviderDictionary.TryGetValue(providerEnum, out var provider) ? provider.CreateSelf() : null;
    }
}