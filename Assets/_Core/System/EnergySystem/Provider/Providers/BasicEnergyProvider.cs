using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnergyProvider : IEnergyProvider
{
    private EnergyConfig _energyConfig;
    public event Action<int> OnEnergyChanged;
    private const string LastGivenDataKey = "LastEnergyGivenTime";
    private const string UnlimitedDataKey = "UnlimitedEnergyEndTime";
    private const string ReloadTimerKey = "EnergyReloadTimer";
    private ulong _unlimitedEnergyEndTime;
    private Dictionary<string, ulong> _stateData = new ();
    public IEnergyProvider CreateSelf()
    {
        return new BasicEnergyProvider();
    }

    private void GetData()
    {
        _stateData = GameInstaller.Instance.SystemLocator.DataManager.GetData<Dictionary<string, ulong>>(GameDataType.State);
    }
    private void SaveData()
    {
        var tempStateData = GameInstaller.Instance.SystemLocator.DataManager.GetData<Dictionary<string, ulong>>(GameDataType.State);
        if (!tempStateData.TryAdd(LastGivenDataKey, _stateData[LastGivenDataKey]))
        {
            tempStateData[LastGivenDataKey]= _stateData[LastGivenDataKey]; 
        }
        if (!tempStateData.TryAdd(UnlimitedDataKey, _stateData.TryGetValue(UnlimitedDataKey, out var value)?value:0))
        {
            tempStateData[UnlimitedDataKey]= _stateData[UnlimitedDataKey]; 
        }
        GameInstaller.Instance.SystemLocator.DataManager.SetData(GameDataType.State, tempStateData);
        GameInstaller.Instance.SystemLocator.DataManager.SaveData();
        
    }
    
    public void Initialize(Action onReady)
    {
#if RemoteConfigManager_Enabled
        _energyConfig = GameInstaller.Instance.SystemLocator.RemoteConfigManager.GetObject<EnergyConfig>();
#else
            _energyConfig = _energyConfig ?? new EnergyConfig();
#endif
        GetData();
        var _lastEnergyGivenTime = TimeHelper.DateTimeToUnixTimeStampInSeconds(GetLastGivenTime());
        var offlineTime = TimeHelper.DateTimeToUnixTimeStampInSeconds(TimeHelper.GetCurrentDateTime()) - _lastEnergyGivenTime;
        int earnedEnergy = (int)(offlineTime / (ulong)_energyConfig.EnergyReloadTimeSecond);
        var givenEnergy = _energyConfig.MaxEnergy - Math.Clamp(Get(),0,_energyConfig.MaxEnergy);
        givenEnergy = earnedEnergy > givenEnergy ? givenEnergy : earnedEnergy;
        if (givenEnergy>0)Add(givenEnergy);
        onReady?.Invoke();
    }

    public void Add(int amount)
    {
        GameInstaller.Instance.SystemLocator.ExchangeManager.DoExchange(CurrencyExtension.GetString(Currency.Energy), amount + 0f);
        var _lastEnergyGivenTime = TimeHelper.DateTimeToUnixTimeStampInSeconds(TimeHelper.GetCurrentDateTime());
        if (!_stateData.TryAdd(LastGivenDataKey, _lastEnergyGivenTime))
        {
            _stateData[LastGivenDataKey] = _lastEnergyGivenTime;
        }
        Debug.Log("Energy Added");
        SaveData();
        if (Get() < _energyConfig.MaxEnergy) StartReloadTimer(_energyConfig.EnergyReloadTimeSecond);
        else if (CoroutineController.IsCoroutineRunning(ReloadTimerKey))
        {
            CoroutineController.StopCoroutine(ReloadTimerKey);
        }
        OnEnergyChanged?.Invoke(Get());
    }

    public void Use(int amount)
    {
        if (!GetUnlimitedEnergyTime().Item1) GameInstaller.Instance.SystemLocator.ExchangeManager.DoExchange(CurrencyExtension.GetString(Currency.Energy), -amount + 0f);
        if (!CoroutineController.IsCoroutineRunning(ReloadTimerKey))
        {
            if (Get() < _energyConfig.MaxEnergy)
            {
                var _lastEnergyGivenTime = TimeHelper.DateTimeToUnixTimeStampInSeconds(TimeHelper.GetCurrentDateTime());
                if (!_stateData.TryAdd(LastGivenDataKey, _lastEnergyGivenTime))
                {
                    _stateData[LastGivenDataKey] = _lastEnergyGivenTime;
                }
                SaveData();
                StartReloadTimer(_energyConfig.EnergyReloadTimeSecond);
            }
        }
        OnEnergyChanged?.Invoke(Get());
    }
    private void StartReloadTimer(float reloadAt = 0f)
    {
        if (CoroutineController.IsCoroutineRunning(ReloadTimerKey))
        {
            CoroutineController.StopCoroutine(ReloadTimerKey);
        }
        CoroutineController.StartCoroutine(ReloadTimerKey,GiveEnergy(reloadAt));
    }

    private IEnumerator GiveEnergy(float reloadAt = 0f)
    {
        yield return new WaitForSeconds(reloadAt);
        if (Get() < _energyConfig.MaxEnergy)
        {
            Add(_energyConfig.WillAddEnergy);
        }
    }

    public int Get()
    {
        return (int)GameInstaller.Instance.SystemLocator.ExchangeManager.GetExchange(CurrencyExtension.GetString(Currency.Energy), 0f);
    }

    public DateTime GetLastGivenTime()
    {
        var stateData = _stateData;
        return stateData.TryGetValue(LastGivenDataKey, out var value)
            ? TimeHelper.UnixTimeStampToDateTime(value)
            : new DateTime();
    }

    public long GetRemainingTime()
    {
        if (GetUnlimitedEnergyTime().Item1)
        {
            return (long)GetUnlimitedEnergyTime().Item2 -
                (long)TimeHelper.DateTimeToUnixTimeStampInSeconds(TimeHelper.GetCurrentDateTime());
        }
        var calculatedTime = (long)(TimeHelper.DateTimeToUnixTimeStampInSeconds(GetLastGivenTime()) + (ulong)_energyConfig.EnergyReloadTimeSecond) -
                             (long)TimeHelper.DateTimeToUnixTimeStampInSeconds(TimeHelper.GetCurrentDateTime());
        if (calculatedTime<0 && Get()<_energyConfig.MaxEnergy)
        {
            Add(1);
            calculatedTime = (long)(TimeHelper.DateTimeToUnixTimeStampInSeconds(GetLastGivenTime()) + (ulong)_energyConfig.EnergyReloadTimeSecond) -
                             (long)TimeHelper.DateTimeToUnixTimeStampInSeconds(TimeHelper.GetCurrentDateTime());
        }
        return calculatedTime;
    }
    public void SetUnlimitedEnergyTime(ulong time)
    {
        var current = GetUnlimitedEnergyTime();
        if (current.Item1)
        {
            _unlimitedEnergyEndTime = current.Item2 + time;
        }else
            _unlimitedEnergyEndTime = TimeHelper.DateTimeToUnixTimeStampInSeconds(TimeHelper.GetCurrentDateTime()) + time;
        if (!_stateData.TryAdd(UnlimitedDataKey, _unlimitedEnergyEndTime))
        {
            _stateData[UnlimitedDataKey] = _unlimitedEnergyEndTime;
        }
        SaveData();
    }
    public (bool, ulong) GetUnlimitedEnergyTime()
    {
        var stateData =
            GameInstaller.Instance.SystemLocator.DataManager.GetData<Dictionary<string, ulong>>(GameDataType.State);
        if ( stateData.TryGetValue(UnlimitedDataKey, out var value))
        {
            return value > TimeHelper.DateTimeToUnixTimeStampInSeconds(TimeHelper.GetCurrentDateTime()) ? (true, value) : (false, 0);
        }
        return (false, 0);
    }
}