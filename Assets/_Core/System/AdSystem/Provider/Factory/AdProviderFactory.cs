using System.Collections.Generic;

public enum AdProviderEnums
{
    TestAdProvider,
#if MaxSdk_Enabled
    ApplovinAdProvider
    #endif
}
public static class AdProviderFactory
{
    private static Dictionary<AdProviderEnums, IAdProvider> _adProviderDictionary = new ()
    {
#if MaxSdk_Enabled
        {AdProviderEnums.ApplovinAdProvider , new ApplovinAdProvider()},
        #endif
        {AdProviderEnums.TestAdProvider, new TestAdProvider()}
    };
    
    public static IAdProvider Create(AdProviderEnums providerEnum)
    {
        return _adProviderDictionary.TryGetValue(providerEnum, out var provider) ? provider.CreateSelf() : null;
    }
}