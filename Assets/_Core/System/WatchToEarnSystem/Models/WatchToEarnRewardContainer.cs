using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WatchToEarnRewardContainer", menuName = "ScriptableObjects/WatchToEarnRewardContainer", order = 1)]
public class WatchToEarnRewardContainer : ScriptableObject
{
    [SerializeField]
    public List<WatchToEarnReward> WatchToEarnRewards;
    
    public WatchToEarnReward GetWatchToEarnReward(int id)
    {
        return id>=WatchToEarnRewards.Count ? null : WatchToEarnRewards[id];
    }
}