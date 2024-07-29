#if !DataManager_Modified
using System;
using System.Collections.Generic;

[Serializable]
public class ProfileSummaryData
{
    #region User Data

    public ProfileModel profile = new ProfileModel();
    public Statistic statistic = new Statistic();

    #endregion

    public void Parse(Dictionary<string, object> data)
    {
        profile = data[nameof(profile)] as ProfileModel;
        statistic = data[nameof(statistic)] as Statistic;
    }

    public Dictionary<string, object> ToDictionary()
    {
        var dict = new Dictionary<string, object>();
        dict.Add(nameof(profile), profile);
        dict.Add(nameof(statistic), statistic);
        return dict;
    }
}

[Serializable]
public class Statistic
{
    public int winCount;
    public int loseCount;
    public int winStreak;
    public int maxTrophy;
    public int currentTrophy;
}
#endif