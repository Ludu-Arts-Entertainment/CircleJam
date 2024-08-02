using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BasicWatchToEarnProvider : IWatchToEarnProvider
{
    public Action<int> OnClaimed { get; set; }
    public Action OnRemained { get; set; }
    private WatchToEarnRewardContainer _watchToEarnRewardContainer;
    
    private const string LastWatchToEarnRewardClaimAt = "LastWatchToEarnRewardClaimAt";
    private const string WatchToEarnClaimCount = "WatchToEarnClaimCount";
    
    private WatchToEarnConfig _watchToEarnConfig;
    
    private CancellationTokenSource _cts;
    public IWatchToEarnProvider CreateSelf()
    {
        return new BasicWatchToEarnProvider();
    }

    public async void Initialize(Action onReady)
    {
        _cts = new CancellationTokenSource();
#if RemoteConfigManager_Enabled
        await UniTask.WaitUntil(()=>GameInstaller.Instance.ManagerDictionary.ContainsKey(ManagerEnums.RemoteConfigManager));
#endif
#if WatchToEarnManager_Enabled
        _watchToEarnConfig = GameInstaller.Instance.SystemLocator.RemoteConfigManager.GetObject<WatchToEarnConfig>();
#endif
        _watchToEarnRewardContainer = Resources.Load<WatchToEarnRewardContainer>(nameof(WatchToEarnRewardContainer));
        
        var remainingTime = GetRemainingTime();
        if (remainingTime == 0)
        {
            Reset();
        }
        UniTask.Delay(remainingTime*1000, cancellationToken: _cts.Token).ContinueWith(Reset);
        onReady?.Invoke();
    }
    public void Claim()
    {
        if (!IsClaimable())return;
        var state = GameInstaller.Instance.SystemLocator.DataManager.GetData<Dictionary<string, ulong>>(GameDataType.State);
        
        if (state.TryGetValue(LastWatchToEarnRewardClaimAt, out var value))
        {
            var currentTimeStamp = TimeHelper.DateTimeToUnixTimeStampInSeconds(TimeHelper.GetCurrentDateTime());

            if(value+ (ulong)_watchToEarnConfig.RefreshTime<=currentTimeStamp)
            {
                state[LastWatchToEarnRewardClaimAt] = currentTimeStamp;
                OnRemained?.Invoke();
            }
        }
        else
        {
            state.Add(LastWatchToEarnRewardClaimAt, TimeHelper.DateTimeToUnixTimeStampInSeconds(TimeHelper.GetCurrentDateTime()));
        }
        
        var reward = GetWatchToEarnReward().ProductBlocks;
        GiverService.Give(reward, ()=>
        {
            state[WatchToEarnClaimCount]+= 1;
            GameInstaller.Instance.SystemLocator.DataManager.SetData(GameDataType.State, state);
            GameInstaller.Instance.SystemLocator.DataManager.SaveData();
            OnClaimed?.Invoke((int)state[WatchToEarnClaimCount]-1);
        });
        
        
    }
    public bool IsClaimable()
    {
        var state = GameInstaller.Instance.SystemLocator.DataManager.GetData<Dictionary<string, ulong>>(GameDataType.State);
        state.TryGetValue(WatchToEarnClaimCount, out var value);
        return GetRemainingTime() >= 0 && value <= (ulong)(_watchToEarnRewardContainer.WatchToEarnRewards.Count);
    }
    public List<WatchToEarnReward> GetWatchToEarnRewards()
    {
        return _watchToEarnRewardContainer != null ? _watchToEarnRewardContainer.WatchToEarnRewards : null;
    }
    public WatchToEarnReward GetWatchToEarnReward(int queue = -1)
    {
        if (queue != -1)
        {
            if (_watchToEarnRewardContainer != null)
            {
                return _watchToEarnRewardContainer.GetWatchToEarnReward(queue);
            }
            return null;
        }
        var state = GameInstaller.Instance.SystemLocator.DataManager.GetData<Dictionary<string, ulong>>(GameDataType.State);
        if (state.TryGetValue(WatchToEarnClaimCount, out var value))
        {
            queue = (int) value;
        }else
            queue = 0;
        return _watchToEarnRewardContainer != null ? _watchToEarnRewardContainer.GetWatchToEarnReward(queue) : null;
    }
    public WatchToEarnRewardStatus GetWatchToEarnRewardStatus(int queue)
    {
        var state = GameInstaller.Instance.SystemLocator.DataManager.GetData<Dictionary<string, ulong>>(GameDataType.State);
        if (!state.TryGetValue(WatchToEarnClaimCount, out var value))
        {
            return queue==0 ? WatchToEarnRewardStatus.Claimable : WatchToEarnRewardStatus.UnClaimable;
        }

        if (value> (ulong)queue)
        {
            return WatchToEarnRewardStatus.Claimed;
        }
        if (value == (ulong)queue)
        {
            return WatchToEarnRewardStatus.Claimable;
        }
        
        return WatchToEarnRewardStatus.UnClaimable;
    }
    public int GetRemainingTime()
    {
        var state = GameInstaller.Instance.SystemLocator.DataManager.GetData<Dictionary<string, ulong>>(GameDataType.State);
        if (!state.TryGetValue(LastWatchToEarnRewardClaimAt, out var value)) return 0;
        var ts = TimeHelper.UnixTimeStampToDateTime(value).AddSeconds(_watchToEarnConfig.RefreshTime) - TimeHelper.GetCurrentDateTime();
        if (ts.TotalSeconds > 0) return (int)ts.TotalSeconds;
        return 0;
    }
    private void Reset()
    {
        var state = GameInstaller.Instance.SystemLocator.DataManager.GetData<Dictionary<string, ulong>>(GameDataType.State);
        if (!state.TryAdd(WatchToEarnClaimCount, 0))
        {
            state[WatchToEarnClaimCount] = 0;
        }
        if (!state.TryAdd(LastWatchToEarnRewardClaimAt, TimeHelper.DateTimeToUnixTimeStampInSeconds(TimeHelper.GetCurrentDateTime())))
        {
            state[LastWatchToEarnRewardClaimAt] = TimeHelper.DateTimeToUnixTimeStampInSeconds(TimeHelper.GetCurrentDateTime());
            OnRemained?.Invoke();
        }
        UniTask.Delay(_watchToEarnConfig.RefreshTime*1000, cancellationToken: _cts.Token).ContinueWith(Reset);
        GameInstaller.Instance.SystemLocator.DataManager.SetData(GameDataType.State, state);
        GameInstaller.Instance.SystemLocator.DataManager.SaveData();
    }
}
