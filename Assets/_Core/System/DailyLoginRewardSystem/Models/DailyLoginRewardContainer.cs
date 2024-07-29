using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "DailyLoginRewardContainer", menuName = "ScriptableObjects/DailyLoginRewardContainer", order = 1)]
public class DailyLoginRewardContainer : ScriptableObject
{
    [SerializeField]
    public List<DailyLoginReward> DailyLoginRewards;
    
    public DailyLoginReward GetDailyLoginRewards(int day)
    {
        return DailyLoginRewards[day%DailyLoginRewards.Count];
    }
}