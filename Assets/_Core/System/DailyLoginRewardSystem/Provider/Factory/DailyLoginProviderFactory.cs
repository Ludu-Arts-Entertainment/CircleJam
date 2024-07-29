using System.Collections.Generic;

public enum DailyLoginProviderEnums
{
    BasicDailyLoginProvider
}

public static class DailyLoginProviderFactory
{
    private static  Dictionary<DailyLoginProviderEnums, IDailyLoginProvider> _dailyLoginProviderDictionary = new ()
    {
        {DailyLoginProviderEnums.BasicDailyLoginProvider, new BasicDailyLoginProvider()}
    };
    
    public static IDailyLoginProvider Create(DailyLoginProviderEnums providerEnum)
    {
        return _dailyLoginProviderDictionary.TryGetValue(providerEnum, out var provider) ? provider.CreateSelf() : null;
    }
}