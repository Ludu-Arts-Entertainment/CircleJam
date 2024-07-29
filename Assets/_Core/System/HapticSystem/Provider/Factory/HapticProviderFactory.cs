using System.Collections.Generic;

public enum HapticProviderEnums
{
    TapticHapticProvider,
}
public static class HapticProviderFactory
{
    private static Dictionary<HapticProviderEnums, IHapticProvider> _hapticProviderDictionary = new ()
    {
        {HapticProviderEnums.TapticHapticProvider , new TapticHapticProvider()},
    };
    
    public static IHapticProvider Create(HapticProviderEnums providerEnum)
    {
        return _hapticProviderDictionary.TryGetValue(providerEnum, out var provider) ? provider.CreateSelf() : null;
    }
}