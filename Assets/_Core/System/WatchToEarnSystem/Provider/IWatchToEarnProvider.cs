using System;
using System.Collections.Generic;

public interface IWatchToEarnProvider
{
    Action<int> OnClaimed { get; set; }
    Action OnRemained { get; set; }
    IWatchToEarnProvider CreateSelf();
    void Initialize(Action onReady);
    void Claim();
    bool IsClaimable();
    List<WatchToEarnReward>  GetWatchToEarnRewards();
    WatchToEarnReward GetWatchToEarnReward(int queue = -1);
    WatchToEarnRewardStatus GetWatchToEarnRewardStatus(int queue);
    int GetRemainingTime();
}

