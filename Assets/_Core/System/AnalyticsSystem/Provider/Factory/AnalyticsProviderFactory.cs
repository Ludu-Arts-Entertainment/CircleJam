using System.Collections.Generic;

public enum AnalyticsProviderEnums
{
    BasicAnalyticsProvider,
}
public static class AnalyticsProviderFactory
{
    private static readonly Dictionary<AnalyticsProviderEnums, IAnalyticsSystemProvider> AnalyticsProviderDictionary = new ()
    {
        {AnalyticsProviderEnums.BasicAnalyticsProvider, new BasicAnalyticsSystemProvider()}
    };
    
    public static IAnalyticsSystemProvider Create(AnalyticsProviderEnums providerEnum)
    {
        return AnalyticsProviderDictionary.TryGetValue(providerEnum, out var provider) ? provider.CreateSelf() : null;
    }
}