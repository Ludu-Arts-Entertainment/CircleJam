using System;
using System.Collections.Generic;
using System.Numerics;

public class ExchangeManager : IManager
{
    private IExchangeProvider _exchangeProvider;

    public IManager CreateSelf()
    {
        return new ExchangeManager();
    }

    public event Action<string, object> OnExchange
    {
        add => _exchangeProvider.OnExchange += value;
        remove => _exchangeProvider.OnExchange -= value;
    }

    public void Initialize(GameInstaller gameInstaller, Action onReady)
    {
        _exchangeProvider = ExchangeProviderFactory.Create(gameInstaller.Customizer.exchangeProvider);
        _exchangeProvider.Initialize(onReady);
    }

    public bool IsReady()
    {
        return _exchangeProvider != null;
    }

    public void Dispose()
    {
        _exchangeProvider = null;
        // TODO: dispose providers
    }

    public void DoExchange(string type, float amount)
    {
        _exchangeProvider.DoExchange(type, amount);

        if (0 > amount)
        {
            TrackingService.Feed(TrackType.CurrencySpend, type, GetExchange(type, 0f));
        }
        else
        {
            TrackingService.Feed(TrackType.CurrencyGained, type, GetExchange(type, 0f));
        }
    }
    
    public float GetExchange(string type, float amount = default)
    {
        return _exchangeProvider.GetExchange(type, amount);
    }

    public float GetExchange(Dictionary<string, TypeStringTuple> sourceData, string type, float amount = default)
    {
        return _exchangeProvider.GetExchange(sourceData, type, amount);
    }

    public void ForceExchange(string type, float amount)
    {
        var oldValue = GetExchange(type, 0f);
        
        _exchangeProvider.ForceExchange(type, amount);
        
        if (oldValue > amount)
        {
            TrackingService.Feed(TrackType.CurrencySpend, type, amount);
        }
        else
        {
            TrackingService.Feed(TrackType.CurrencyGained, type, amount);
        }
    }
}
