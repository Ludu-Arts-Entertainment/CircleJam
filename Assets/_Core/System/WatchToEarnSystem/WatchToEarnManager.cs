using System;
using System.Collections.Generic;

public class WatchToEarnManager : IManager
{
    public event Action<int> OnClaimed
    {
        add => _watchToEarnProvider.OnClaimed += value;
        remove => _watchToEarnProvider.OnClaimed -= value;
    }
    public event Action OnRemained
    {
        add => _watchToEarnProvider.OnRemained += value;
        remove => _watchToEarnProvider.OnRemained -= value;
    }
    private IWatchToEarnProvider _watchToEarnProvider;
    public IManager CreateSelf()
    {
        return new WatchToEarnManager();
    }

    public void Initialize(GameInstaller gameInstaller, Action onReady)
    {
        _watchToEarnProvider = WatchToEarnProviderFactory.Create(gameInstaller.Customizer.WatchToEarnProviderType);
        _watchToEarnProvider.Initialize(onReady);
    }

    public bool IsReady()
    {
        return _watchToEarnProvider != null;
    }

    public List<WatchToEarnReward> GetWatchToEarnRewards()
    {
        return _watchToEarnProvider.GetWatchToEarnRewards();
    }

    public WatchToEarnRewardStatus GetWatchToEarnRewardStatus(int queue)
    {
        return _watchToEarnProvider.GetWatchToEarnRewardStatus(queue);
    }

    public bool IsClaimable()
    {
        return _watchToEarnProvider.IsClaimable();
    }

    public void Claim()
    {
        _watchToEarnProvider.Claim();
    }
    public WatchToEarnReward GetWatchToEarnReward(int queue = -1)
    {
        return _watchToEarnProvider.GetWatchToEarnReward(queue);
    }
    public int GetRemainingTime()
    {
        return _watchToEarnProvider.GetRemainingTime();
    }

}