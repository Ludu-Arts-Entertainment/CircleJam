using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FakeLeaderboardProvider : ILeaderboardProvider
{
    public ILeaderboardProvider CreateSelf()
    {
        return new FakeLeaderboardProvider();
    }

    public void Initialize(Action onReady)
    {
        onReady?.Invoke();
    }

    public void SendStat(string statName, int statValue)
    {
        Debug.Log("New stat sent to leaderboard");
    }

    public void SendStats(Dictionary<string, object> stats)
    {
        Debug.Log("New stats sent to leaderboard");
    }

    public void SendRequestForGetPlayerRankFromLeaderboard(string leaderboardId, Action<ILeaderboardPlayer> onResultCallback, Action<object> onErrorCallback)
    {
        Debug.Log("Request for player rank from leaderboard sent");
    }

    public void SendRequestForGetLeaderboardAroundPlayer(string leaderboardId, int maxResultCount, Action<List<ILeaderboardPlayer>> onResultCallback,
        Action<object> onErrorCallback)
    {
        Debug.Log("Request for leaderboard around player sent");
    }

    public void SendRequestForGetLeaderboard(string leaderboardId, int start, int count, Action<List<ILeaderboardPlayer>> onResultCallback,
        Action<object> onErrorCallback)
    {
        Debug.Log("Request for leaderboard sent");
    }

    public Task<ILeaderboardPlayer> GetOwnPlayer(string leaderboardId)
    {
        throw new NotImplementedException();
    }

    public ILeaderboardPlayer[] GetPlayers(string leaderboardId, int start, int count)
    {
        throw new NotImplementedException();
    }
}