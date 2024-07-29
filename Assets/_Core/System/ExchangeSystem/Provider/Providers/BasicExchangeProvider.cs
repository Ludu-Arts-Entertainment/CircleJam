using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class BasicExchangeProvider : IExchangeProvider
{
    private Dictionary<string, TypeStringTuple> _exchangeData = new();

    public event Action<string, object> OnExchange;

    public IExchangeProvider CreateSelf()
    {
        return new BasicExchangeProvider();
    }

    public void Initialize(Action onReady)
    {
        Load();
        onReady?.Invoke();
    }

    private void Load()
    {
        _exchangeData =
            GameInstaller.Instance.SystemLocator.DataManager.GetData<Dictionary<string, TypeStringTuple>>(
                GameDataType.ExchangeData);
    }

    private void Save()
    {
        GameInstaller.Instance.SystemLocator.DataManager.SetData(GameDataType.ExchangeData, _exchangeData);
        GameInstaller.Instance.SystemLocator.DataManager.SaveData();
    }

    public void DoExchange(string type, BigInteger amount)
    {
        if (!_exchangeData.TryAdd(type, new TypeStringTuple(typeof(BigInteger), JsonHelper.ToJson(amount))))
        {
            try
            {
                var h = JsonHelper.FromJson<BigInteger>(_exchangeData[type].value);
                h += amount;
                _exchangeData[type].value = JsonHelper.ToJson(h);
            }
            catch (Exception e)
            {
                _exchangeData[type].Type = typeof(BigInteger);
                _exchangeData[type].value = JsonHelper.ToJson(amount);
                Debug.LogWarning(e);
            }
        }

        Save();
        OnExchange?.Invoke(type, (long)amount);
    }

    public void DoExchange(string type, float amount)
    {
        if (!_exchangeData.TryAdd(type, new TypeStringTuple(typeof(float), JsonHelper.ToJson(amount))))
        {
            try
            {
                var h = JsonHelper.FromJson<float>(_exchangeData[type].value);
                h += amount;
                _exchangeData[type].value = JsonHelper.ToJson(h);
            }
            catch (Exception e)
            {
                _exchangeData[type].Type = typeof(float);
                _exchangeData[type].value = JsonHelper.ToJson(amount);
                Debug.LogWarning(e);
            }
        }

        Save();
        OnExchange?.Invoke(type, (long)amount);
    }

    public BigInteger GetExchange(string type, BigInteger amount = default)
    {
        return _exchangeData.TryGetValue(type, out var result) ? JsonHelper.FromJson<BigInteger>(result.value) : amount;
    }

    public float GetExchange(string type, float amount = default)
    {
        return _exchangeData.TryGetValue(type, out var result) ? JsonHelper.FromJson<float>(result.value) : amount;
    }

    public float GetExchange(Dictionary<string, TypeStringTuple> sourceData, string type, float amount = default)
    {
        return sourceData.TryGetValue(type, out var result) ? JsonHelper.FromJson<float>(result.value) : amount;
    }

    public void ForceExchange(string type, float amount)
    {
        if (!_exchangeData.TryAdd(type, new TypeStringTuple(typeof(float), JsonHelper.ToJson(amount))))
        {
            _exchangeData[type].Type = typeof(float);
            _exchangeData[type].value = JsonHelper.ToJson(amount);
        }

        Save();
        OnExchange?.Invoke(type,(long)amount);
    }

    public void ForceExchange(string type, BigInteger amount)
    {
        if (!_exchangeData.TryAdd(type, new TypeStringTuple(typeof(BigInteger), JsonHelper.ToJson(amount))))
        {
            _exchangeData[type].Type = typeof(BigInteger);
            _exchangeData[type].value = JsonHelper.ToJson(amount);
        }

        Save();
        OnExchange?.Invoke(type,(long)amount);
    }
}

public record TypeStringTuple
{
    public Type Type;
    public string value;

    public TypeStringTuple(Type type, string value)
    {
        Type = type;
        this.value = value;
    }
}