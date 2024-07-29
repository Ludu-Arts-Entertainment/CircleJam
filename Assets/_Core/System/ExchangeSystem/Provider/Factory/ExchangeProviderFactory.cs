using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ExchangeProviderEnums
{
    BasicExchangeProvider,
}
public static class ExchangeProviderFactory
{
    private static Dictionary<ExchangeProviderEnums,IExchangeProvider> _exchangeProviders = new Dictionary<ExchangeProviderEnums, IExchangeProvider>()
    {
        {ExchangeProviderEnums.BasicExchangeProvider,new BasicExchangeProvider()}
    };
    public static IExchangeProvider Create(ExchangeProviderEnums providerEnum)
    {
        return _exchangeProviders.TryGetValue(providerEnum, out var provider) ? provider.CreateSelf() : null;
    }
}
