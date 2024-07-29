using System.Collections.Generic;

public enum UIProviderEnums
{
    BasicUIProvider,
}
public static class UIProviderFactory
{
    private static Dictionary<UIProviderEnums,IUIProvider> _uiProviderDictionary = new ()
    {
        {UIProviderEnums.BasicUIProvider, new BasicUIProvider()},
    };
    
    public static IUIProvider Create(UIProviderEnums providerEnum)
    {
        return _uiProviderDictionary.TryGetValue(providerEnum, out var provider) ? provider.CreateSelf() : null;
    }
}
