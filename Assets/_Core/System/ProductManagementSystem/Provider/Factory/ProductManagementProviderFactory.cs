using System.Collections.Generic;

public enum ProductManagementProviderEnums
{
    BasicProductManagementProvider,
#if IAPManager_Enabled
    IAPProductManagementProvider,
#endif
}

public static class ProductManagementProviderFactory
{
    private static Dictionary<ProductManagementProviderEnums, IProductManagementProvider> _providers = new()
    {
        { ProductManagementProviderEnums.BasicProductManagementProvider, new BasicProductManagementProvider() },
#if IAPManager_Enabled
        { ProductManagementProviderEnums.IAPProductManagementProvider, new IAPProductManagementProvider() },
#endif
    };
    
    public static IProductManagementProvider Create(ProductManagementProviderEnums providerEnum)
    {
        return _providers.TryGetValue(providerEnum, out var provider) ? provider.CreateSelf() : null;
    }
}
