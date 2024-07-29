using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class LeaderboardManager : IManager
{
    public const int LEADERBOARD_UNLOCK_TROPHY_COUNT = 5;
    private ILeaderboardProvider _leaderboardProvider;
    private bool isUnlocked;
    public IManager CreateSelf()
    {
        return new LeaderboardManager();
    }

    public async void Initialize(GameInstaller gameInstaller, Action onReady)
    {
        await UniTask.WaitUntil(()=>GameInstaller.Instance.ManagerDictionary.ContainsKey(ManagerEnums.ExchangeManager));
        _leaderboardProvider = LeaderboardProviderFactory.Create(gameInstaller.Customizer.LeaderboardProviderEnum);
        _leaderboardProvider.Initialize(onReady);

        LoadLockedData();
        GameInstaller.Instance.SystemLocator.ExchangeManager.OnExchange += OnExchangeChanged;
    }

    private void LoadLockedData()
    {
        var stateData = GameInstaller.Instance.SystemLocator.DataManager.GetData<Dictionary<string, ulong>>(GameDataType.State);
        if (stateData.TryGetValue("IsLeaderboardUnlocked", out var value))
        {
            isUnlocked = value == 1;
        }
        else
        {
            // var trophyCount = GameInstaller.Instance.SystemLocator.ExchangeManager.GetExchange(Currency.Trophy, 0f);
            // isUnlocked = trophyCount >= LEADERBOARD_UNLOCK_TROPHY_COUNT;
            // stateData.Add("IsLeaderboardUnlocked", isUnlocked == true ? 1 : 0);
            // GameInstaller.Instance.SystemLocator.DataManager.SetData(GameDataType.State, stateData);
        }
    }

    private void OnExchangeChanged(string arg1, object arg2)
    {
        // if(arg1 == Currency.Trophy)
        // {
        //     CheckLeaderboardUnlock();
        // }
    }

    public void CheckLeaderboardUnlock()
    {
        // var trophyCount = GameInstaller.Instance.SystemLocator.ExchangeManager.GetExchange(Currency.Trophy, 0f);
        // if (trophyCount >= LEADERBOARD_UNLOCK_TROPHY_COUNT)
        // {
        //     isUnlocked = true;
        //     var stateData = GameInstaller.Instance.SystemLocator.DataManager.GetData<Dictionary<string, int>>(GameDataType.State);
        //     stateData["IsLeaderboardUnlocked"] = 1;
        //     GameInstaller.Instance.SystemLocator.DataManager.SetData(GameDataType.State, stateData);
        // }
    }

    public bool IsLeaderboardUnlocked()
    {
        return isUnlocked;
    }

    public bool IsReady()
    {
        return _leaderboardProvider != null;
    }

    public void SendStat(string statName, int statValue)
    {
        _leaderboardProvider.SendStat(statName, statValue);
    }
    
    public void SendStats(Dictionary<string, object> stats)
    {
        _leaderboardProvider.SendStats(stats);
    }

    public void SendRequestForGetPlayerRankFromLeaderboard(string leaderboardId, Action<ILeaderboardPlayer> onResultCallback, Action<object> onErrorCallback)
    {
        _leaderboardProvider.SendRequestForGetPlayerRankFromLeaderboard(leaderboardId, onResultCallback, onErrorCallback);
    }

    public void SendRequestForGetLeaderboardAroundPlayer(string leaderboardId, int maxResultCount, Action<List<ILeaderboardPlayer>> onResultCallback, Action<object> onErrorCallback)
    {
        _leaderboardProvider.SendRequestForGetLeaderboardAroundPlayer(leaderboardId, maxResultCount, onResultCallback, onErrorCallback);
    }

    public void SendRequestForGetLeaderboard(string leaderboardId, int start, int count, Action<List<ILeaderboardPlayer>> onResultCallback, Action<object> onErrorCallback)
    {
        _leaderboardProvider.SendRequestForGetLeaderboard(leaderboardId, start, count, onResultCallback, onErrorCallback);
    }
}
