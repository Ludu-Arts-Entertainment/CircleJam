using System.Collections.Generic;

public enum DataProviderEnums
{
    PlayerPrefsDataProvider,
#if PlayFabSdk_Enabled
    PlayFabDataProvider,
#endif
}

public static class DataProviderFactory
{
    private static Dictionary<DataProviderEnums,IDataProvider> _dataProviderDictionary = new Dictionary<DataProviderEnums, IDataProvider>()
    {
        {DataProviderEnums.PlayerPrefsDataProvider, new PlayerPrefsDataProvider()},
#if PlayFabSdk_Enabled
        {DataProviderEnums.PlayFabDataProvider, new PlayFabDataProvider()},
#endif
    };
    
    public static IDataProvider Create(DataProviderEnums providerEnum)
    {
        return _dataProviderDictionary.TryGetValue(providerEnum, out var provider) ? provider.CreateSelf() : null;
    }
}
