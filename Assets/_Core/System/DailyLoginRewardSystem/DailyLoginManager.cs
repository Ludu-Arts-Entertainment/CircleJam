using System;
using System.Collections.Generic;

public class DailyLoginManager : IManager
{
    private IDailyLoginProvider _dailyLoginProvider;
    public event Action<int> OnClaimed
    {
        add => _dailyLoginProvider.OnClaimed += value;
        remove => _dailyLoginProvider.OnClaimed -= value;
    }
    public IManager CreateSelf()
    {
        return new DailyLoginManager();
    }

    public void Initialize(GameInstaller gameInstaller, Action onReady)
    {
        _dailyLoginProvider = DailyLoginProviderFactory.Create(gameInstaller.Customizer.DailyLoginProviderEnum);
        _dailyLoginProvider.Initialize(onReady);
    }

    public bool IsReady()
    {
        return _dailyLoginProvider != null;
    }
    public void Claim()
    {
        _dailyLoginProvider.Claim();
    }
    public bool IsClaimable()
    {
        return _dailyLoginProvider.IsClaimable();
    }
    public List<DailyLoginReward> GetDailyLoginRewards()
    {
        return _dailyLoginProvider.GetDailyLoginRewards();
    }
    public DailyLoginReward GetDailyLoginReward(int day = -1)
    {
        return _dailyLoginProvider.GetDailyLoginReward(day);
    }
    public DailyLoginRewardStatus GetDailyLoginRewardStatus(int day)
    {
        return _dailyLoginProvider.GetDailyLoginRewardStatus(day);
    }
    public TimeSpan GetNextClaimTime()
    {
        return _dailyLoginProvider.GetNextClaimTime();
    }
}
