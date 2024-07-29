using System.Collections.Generic;

public enum InventoryProviderEnums
{
    BasicInventoryProvider,
}
public static class InventoryProviderFactory
{
    private static Dictionary<InventoryProviderEnums, IInventoryProvider> _inventoryProviderDictionary = new ()
    {
        {InventoryProviderEnums.BasicInventoryProvider, new BasicInventoryProvider()},
    };

    public static IInventoryProvider Create(InventoryProviderEnums providerEnum)
    {
        return _inventoryProviderDictionary.TryGetValue(providerEnum, out var provider) ? provider.CreateSelf() : null;
    }
    
}
