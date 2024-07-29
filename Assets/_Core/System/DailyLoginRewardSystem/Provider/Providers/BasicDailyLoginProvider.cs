using System;
using System.Collections.Generic;
using UnityEngine;

internal class BasicDailyLoginProvider : IDailyLoginProvider
{
    private DailyLoginRewardContainer _dailyLoginRewardContainer;
    public Action<int> OnClaimed { get; set; }

    public IDailyLoginProvider CreateSelf()
    {
        return new BasicDailyLoginProvider();
    }

    public void Initialize(Action onReady)
    {
        _dailyLoginRewardContainer = Resources.Load<DailyLoginRewardContainer>(nameof(DailyLoginRewardContainer));
        onReady?.Invoke();
    }
    
    public void Claim()
    {
        if (!IsClaimable())return;
        var state = GameInstaller.Instance.SystemLocator.DataManager.GetData<Dictionary<string, ulong>>(GameDataType.State);
        var currentTimeStamp = TimeHelper.DateTimeToUnixTimeStampInSeconds(TimeHelper.GetCurrentDateTime().Date);
        if (!state.TryAdd("LastDailyLoginClaimAt", currentTimeStamp))
        {
            state["LastDailyLoginClaimAt"] = currentTimeStamp;
        }
        if (!state.TryAdd("DailyLoginClaimCount", 1))
        {
            state["DailyLoginClaimCount"]++;
        }
        GiverService.Give(GetDailyLoginReward().ProductBlocks, ()=>
        {
            OnClaimed?.Invoke((int)state["DailyLoginClaimCount"]-1);
        });
        GameInstaller.Instance.SystemLocator.DataManager.SetData(GameDataType.State, state);
        GameInstaller.Instance.SystemLocator.DataManager.SaveData();
    }
    public bool IsClaimable()
    {
        return GetNextClaimTime()==TimeSpan.Zero;
    }
    public List<DailyLoginReward> GetDailyLoginRewards()
    {
        return _dailyLoginRewardContainer != null ? _dailyLoginRewardContainer.DailyLoginRewards : null;
    }
    public DailyLoginReward GetDailyLoginReward(int day = -1)
    {
        if (day != -1) return _dailyLoginRewardContainer != null ? _dailyLoginRewardContainer.GetDailyLoginRewards(day) : null;
        var state = GameInstaller.Instance.SystemLocator.DataManager.GetData<Dictionary<string, ulong>>(GameDataType.State);
        if (state.TryGetValue("DailyLoginClaimCount", out var value))
        {
            day = (int) value;
        }else
            day = 0;
        return _dailyLoginRewardContainer != null ? _dailyLoginRewardContainer.GetDailyLoginRewards(day) : null;
    }
    public DailyLoginRewardStatus GetDailyLoginRewardStatus(int day)
    {
        var state = GameInstaller.Instance.SystemLocator.DataManager.GetData<Dictionary<string, ulong>>(GameDataType.State);
        if (!state.TryGetValue("DailyLoginClaimCount", out var value))
        {
            if(day==0)
                return DailyLoginRewardStatus.Claimable;
        }
        day = day%_dailyLoginRewardContainer.DailyLoginRewards.Count;
        var intValue = (int)value % _dailyLoginRewardContainer.DailyLoginRewards.Count;
        if(day<intValue) return DailyLoginRewardStatus.Claimed;
        return day==intValue ? DailyLoginRewardStatus.Claimable : DailyLoginRewardStatus.UnClaimable;
    }
    public TimeSpan GetNextClaimTime()
    {
        var state = GameInstaller.Instance.SystemLocator.DataManager.GetData<Dictionary<string, ulong>>(GameDataType.State);
        if (!state.TryGetValue("LastDailyLoginClaimAt", out var value)) return TimeSpan.Zero;
        var ts = TimeHelper.UnixTimeStampToDateTime(value).AddDays(1) - TimeHelper.GetCurrentDateTime();
        if (ts.Days > 0) return TimeSpan.Zero;
        if (ts<TimeSpan.Zero)return TimeSpan.Zero;
        return ts;
    }
}

public enum DailyLoginRewardStatus
{
    Claimed,
    Claimable,
    UnClaimable
}