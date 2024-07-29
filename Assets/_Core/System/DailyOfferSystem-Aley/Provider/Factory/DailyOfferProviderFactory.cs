using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DailyOfferProviderEnums
{
    DailyOfferProvider
}
public static class DailyOfferProviderFactory
{
    private static Dictionary<DailyOfferProviderEnums,IDailyOfferProvider> _dailyOfferProviderDictionary = new ()
    {
        {DailyOfferProviderEnums.DailyOfferProvider, new BaseDailyOfferProvider()},
    };
    
    public static IDailyOfferProvider Create(DailyOfferProviderEnums providerEnum)
    {
        return _dailyOfferProviderDictionary.TryGetValue(providerEnum, out var provider) ? provider.CreateSelf() : null;
    }
}
