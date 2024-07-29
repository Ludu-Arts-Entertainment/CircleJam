using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RouletteProviderEnums
{
    BaseRouletteProvider,
}
public static class RouletteProviderFactory
{
    // Start is called before the first frame update
    private static Dictionary<RouletteProviderEnums,IRouletteProvider> _rouletteProviderDictionary = new ()
    {
        {RouletteProviderEnums.BaseRouletteProvider, new BaseRouletteProvider()},
    };
    
    public static IRouletteProvider Create(RouletteProviderEnums providerEnum)
    {
        return _rouletteProviderDictionary.TryGetValue(providerEnum, out var provider) ? provider.CreateSelf() : null;
    }
}

