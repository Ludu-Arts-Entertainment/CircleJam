using System;
using System.Collections.Generic;
using System.Numerics;

public interface IExchangeProvider
{
    public event Action<string,object> OnExchange;
    IExchangeProvider CreateSelf();
    void Initialize(Action onReady);
    void DoExchange(string type, float amount);
    float GetExchange(Dictionary<string, TypeStringTuple> sourceData, string type, float amount = default);
    float GetExchange(string type, float amount = default);
    void ForceExchange(string type, float amount);
}
