using System.Collections.Generic;

public enum SettingProviderEnums
{
    BasicSettingProvider
}
public static class SettingProviderFactory
{
    private static Dictionary<SettingProviderEnums, ISettingProvider> _settingProviderDictionary = new ()
    {
        {SettingProviderEnums.BasicSettingProvider , new BasicSettingProvider()},
    };
    
    public static ISettingProvider Create(SettingProviderEnums providerEnum)
    {
        return _settingProviderDictionary.TryGetValue(providerEnum, out var provider) ? provider.CreateSelf() : null;
    }
}