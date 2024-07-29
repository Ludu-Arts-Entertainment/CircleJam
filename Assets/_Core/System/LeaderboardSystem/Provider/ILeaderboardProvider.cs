using System;
using System.Collections.Generic;

public interface ILeaderboardProvider
{
    ILeaderboardProvider CreateSelf();
    void Initialize(Action onReady);
    
    /// <summary>
    /// This function is used to send the stat value to the stat-name based leaderboard.
    /// </summary>
    /// <param name="statName">is used to decide which statName based leaderboard will update with statValue.</param>
    /// <param name="statValue">is used to update statName based value</param>
    void SendStat(string statName, int statValue);
    
    /// <summary>
    /// This function is used to send the stat value to the stat-name based leaderboard.
    /// </summary>
    /// <param name="statName">is used to decide which statName based leaderboard will update with statValue.</param>
    /// <param name="statValue">is used to update statName based value</param>
    void SendStats(Dictionary<string, object> stats);
    
    /// <summary>
    /// This function is used to get the logged in player's rank from the leaderboard.
    /// </summary>
    /// <param name="leaderboardId"> is the stat name parameter for selecting leaderboard.</param>
    /// <param name="onResultCallback">is onsuccess callback that returns ILeaderboardPlayer.</param>
    /// <param name="onErrorCallback">is onerror callback that returns error report.</param>
    void SendRequestForGetPlayerRankFromLeaderboard(string leaderboardId, Action<ILeaderboardPlayer> onResultCallback, Action<object> onErrorCallback);
    
    /// <summary>
    /// This function is used to get the leaderboard around the logged in player.
    /// </summary>
    /// <param name="leaderboardId"> is the stat name parameter for selecting leaderboard.</param>
    /// <param name="maxResultCount">is max entry count of ILeaderboardPlayer list.</param>
    /// <param name="onResultCallback">is onsuccess callback that returns List of ILeaderboardPlayer.</param>
    /// <param name="onErrorCallback">is onerror callback that returns error report.</param>
    void SendRequestForGetLeaderboardAroundPlayer (string leaderboardId, int maxResultCount, Action<List<ILeaderboardPlayer>> onResultCallback, Action<object> onErrorCallback);
    
    /// <summary>
    /// This function is used to get the leaderboard. In other words, it is used to get the top n-players from the leaderboard.
    /// </summary>
    /// <param name="leaderboardId"></param>
    /// <param name="start">is the starting position of leaderboard.</param>
    /// <param name="count">is the entry count after starting position.</param>
    /// <param name="onResultCallback">is onsuccess callback that returns List of ILeaderboardPlayer.</param>
    /// <param name="onErrorCallback">is onerror callback that returns error report.</param>
    void SendRequestForGetLeaderboard(string leaderboardId, int start, int count, Action<List<ILeaderboardPlayer>> onResultCallback, Action<object> onErrorCallback);
}