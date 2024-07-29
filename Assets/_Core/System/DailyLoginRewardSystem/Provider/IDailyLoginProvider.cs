using System;
using System.Collections.Generic;

public interface IDailyLoginProvider
{
    Action<int> OnClaimed { get; set; }
    IDailyLoginProvider CreateSelf();
    void Initialize(Action onReady);
    void Claim();
    bool IsClaimable();
    List<DailyLoginReward>  GetDailyLoginRewards();
    DailyLoginReward GetDailyLoginReward(int day = -1);
    DailyLoginRewardStatus GetDailyLoginRewardStatus(int day);
    TimeSpan GetNextClaimTime();
}
