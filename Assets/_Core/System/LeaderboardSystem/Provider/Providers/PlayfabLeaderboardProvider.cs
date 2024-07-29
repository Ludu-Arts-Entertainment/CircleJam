#if PlayFabSdk_Enabled

using System;
using System.Collections.Generic;
// using Hellmade.Net;
using PlayFab;
using PlayFab.ClientModels;

public class PlayfabLeaderboardProvider : ILeaderboardProvider
{
    private Dictionary<PlayerStatType, List<PlayerLeaderboardEntry>> _leaderboardDictionary;
    private LoginManager _loginManager;

    private PlayerProfileViewConstraints _profileConstraints = new PlayerProfileViewConstraints()
    {
        ShowAvatarUrl = true,
        ShowDisplayName = true,
        ShowLocations = true,
        ShowStatistics = true,
        ShowLastLogin = true
    };


    public ILeaderboardProvider CreateSelf()
    {
        return new PlayfabLeaderboardProvider();
    }

    public  void Initialize(Action onReady)
    {
        _loginManager = GameInstaller.Instance.SystemLocator.LoginManager;
        _leaderboardDictionary = new Dictionary<PlayerStatType, List<PlayerLeaderboardEntry>>();
        GameInstaller.Instance.SystemLocator.ExchangeManager.OnExchange += OnExchangeChanged;
        _isAroundPlayerCached = false;
        onReady?.Invoke();
    }

    private void OnExchangeChanged(string arg1, object arg2)
    {
        // if (arg1 == Currency.Trophy)
        // {
        //     var amount = GameInstaller.Instance.SystemLocator.ExchangeManager.GetExchange(arg1, 0f);
        //     PlayFabHelper.UpdateStatistic("trophy_world", (int)amount);
        // }
    }

    public void SendStat(string statName, int statValue)
    {
        var countryCodeExtension = _loginManager.GetCountryCodeExtension(PlayFabHelper.CountryCode);
        var globalCountryCodeExtension = _loginManager.GetGlobalCountryCodeExtension();

        var newCountryStatName = $"{statName}_{countryCodeExtension}".ToLower();
        var newGlobalStatName = $"{statName}_{globalCountryCodeExtension}".ToLower();

        PlayFabHelper.UpdateStatistic(newCountryStatName, statValue);
        PlayFabHelper.UpdateStatistic(newGlobalStatName, statValue);
    }

    public void SendStats(Dictionary<string, object> stats)
    {
        foreach (var stat in stats)
        {
            var countryCodeExtension = _loginManager.GetCountryCodeExtension(PlayFabHelper.CountryCode);
            var globalCountryCodeExtension = _loginManager.GetGlobalCountryCodeExtension();

            var newCountryStatName = $"{stat.Key}_{countryCodeExtension}".ToLower();
            var newGlobalStatName = $"{stat.Key}_{globalCountryCodeExtension}".ToLower();

            stats.Remove(stat.Key);
            stats.Add(newCountryStatName, stat.Value);
            stats.Add(newGlobalStatName, stat.Value);
        }

        PlayFabHelper.UpdateStatistics(stats);
    }

    public async void SendRequestForGetPlayerRankFromLeaderboard(string leaderboardId,
        Action<ILeaderboardPlayer> onResultCallback, Action<object> onErrorCallback)
    {
        GetLeaderboardAroundPlayerRequest request = new GetLeaderboardAroundPlayerRequest
        {
            MaxResultsCount = 1,
            StatisticName = leaderboardId,
            ProfileConstraints = _profileConstraints,
        };

        // var internetStatus = EazyNetChecker.Status;
        // if(internetStatus == NetStatus.NoDNSConnection) return;

        if (_loginManager.IsLoggedIn == false) return;

        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, resultCallback: OnResultCallback,
            errorCallback: OnErrorCallback);

        void OnResultCallback(GetLeaderboardAroundPlayerResult result)
        {
            if (result == null)
            {
                return;
            }

            if (result.Leaderboard == null || result.Leaderboard.Count == 0)
            {
                return;
            }

            var sentLeaderboardRequest = result.Request as GetLeaderboardAroundPlayerRequest;

            var leaderboardEntry = result.Leaderboard[0];

            var leaderboardPlayer = new LeaderboardPlayer(leaderboardEntry.PlayFabId,
                leaderboardEntry.DisplayName ??= leaderboardEntry.Profile.PlayerId,
                leaderboardEntry.Profile.AvatarUrl ??= "0", (uint)leaderboardEntry.Position + 1);
            leaderboardPlayer.AddStat(sentLeaderboardRequest?.StatisticName, leaderboardEntry.StatValue.ToString());

            onResultCallback?.Invoke(leaderboardPlayer);
        }

        void OnErrorCallback(PlayFabError error)
        {
            onErrorCallback?.Invoke(error.GenerateErrorReport());
        }
    }

    private class PlayerLeaderboard
    {
        public List<ILeaderboardPlayer> Leaderboard { get; set; }
        public double Time;
    }

    private PlayerLeaderboard _playerLeaderboard;
    private bool _isAroundPlayerCached;
    public static int AroundCounter = 0;

    public async void SendRequestForGetLeaderboardAroundPlayer(string leaderboardId, int maxResultCount,
        Action<List<ILeaderboardPlayer>> onResultCallback, Action<object> onErrorCallback)
    {
        GetLeaderboardAroundPlayerRequest request = new GetLeaderboardAroundPlayerRequest
        {
            MaxResultsCount = maxResultCount,
            StatisticName = leaderboardId,
            ProfileConstraints = _profileConstraints,
        };
        //
        // var internetStatus = EazyNetChecker.Status;
        // if(internetStatus == NetStatus.NoDNSConnection) return;

        if (_loginManager.IsLoggedIn == false) return;
        if (_isAroundPlayerCached)
        {
            var currentTime = DateTime.UtcNow.TimeOfDay.TotalSeconds;
            if (currentTime - _playerLeaderboard.Time < 60)
            {
                onResultCallback?.Invoke(_playerLeaderboard.Leaderboard);
                return;
            }
        }

        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, resultCallback: OnResultCallback,
            errorCallback: OnErrorCallback);

        void OnResultCallback(GetLeaderboardAroundPlayerResult result)
        {
            if (result == null)
            {
                return;
            }

            if (result.Leaderboard == null || result.Leaderboard.Count == 0)
            {
                return;
            }

            var sentLeaderboardRequest = result.Request as GetLeaderboardAroundPlayerRequest;
            var leaderboardPlayers = new List<ILeaderboardPlayer>();

            foreach (var leaderboardEntry in result.Leaderboard)
            {
                var leaderboardPlayer = new LeaderboardPlayer(leaderboardEntry.PlayFabId,
                    leaderboardEntry.DisplayName ??= leaderboardEntry.Profile.PlayerId,
                    leaderboardEntry.Profile.AvatarUrl ??= "0", (uint)leaderboardEntry.Position + 1);
                leaderboardPlayer.AddStat(sentLeaderboardRequest?.StatisticName, leaderboardEntry.StatValue.ToString());
                leaderboardPlayers.Add(leaderboardPlayer);
            }

            _playerLeaderboard = new PlayerLeaderboard
            {
                Leaderboard = leaderboardPlayers,
                Time = DateTime.UtcNow.TimeOfDay.TotalSeconds
            };
            _isAroundPlayerCached = true;
            AroundCounter++;
            onResultCallback?.Invoke(_playerLeaderboard.Leaderboard);
        }


        void OnErrorCallback(PlayFabError error)
        {
            onErrorCallback?.Invoke(error.GenerateErrorReport());
        }
    }


    private PlayerLeaderboard _topLeaderboard;
    private bool _isTopLeaderboardCached;
    public static int TopCounter = 0;


    public void SendRequestForGetLeaderboard(string leaderboardId, int start, int count,
        Action<List<ILeaderboardPlayer>> onResultCallback, Action<object> onErrorCallback)
    {
        GetLeaderboardRequest request = new GetLeaderboardRequest
        {
            MaxResultsCount = count,
            StartPosition = start,
            StatisticName = leaderboardId,
            ProfileConstraints = _profileConstraints,
        };
        //
        // var internetStatus = EazyNetChecker.Status;
        // if(internetStatus == NetStatus.NoDNSConnection) return;

        if (_loginManager.IsLoggedIn == false) return;
        if (_isTopLeaderboardCached)
        {
            var currentTime = DateTime.UtcNow.TimeOfDay.TotalSeconds;
            if (currentTime - _topLeaderboard.Time < 60)
            {
                onResultCallback?.Invoke(_topLeaderboard.Leaderboard);
                return;
            }
        }


        PlayFabClientAPI.GetLeaderboard(request, resultCallback: OnResultCallback, errorCallback: OnErrorCallback);

        void OnResultCallback(GetLeaderboardResult result)
        {
            if (result == null)
            {
                return;
            }

            if (result.Leaderboard == null || result.Leaderboard.Count == 0)
            {
                return;
            }

            var sentLeaderboardRequest = result.Request as GetLeaderboardRequest;

            var leaderboardPlayers = new List<ILeaderboardPlayer>();

            foreach (var leaderboardEntry in result.Leaderboard)
            {
                var leaderboardPlayer = new LeaderboardPlayer(leaderboardEntry.PlayFabId,
                    leaderboardEntry.DisplayName ??= leaderboardEntry.Profile.PlayerId,
                    leaderboardEntry.Profile.AvatarUrl ??= "0", (uint)leaderboardEntry.Position + 1);
                leaderboardPlayer.AddStat(sentLeaderboardRequest?.StatisticName, leaderboardEntry.StatValue.ToString());
                leaderboardPlayers.Add(leaderboardPlayer);
            }

            _topLeaderboard = new PlayerLeaderboard()
            {
                Leaderboard = leaderboardPlayers,
                Time = DateTime.UtcNow.TimeOfDay.TotalSeconds
            };
            _isTopLeaderboardCached = true;
            TopCounter++;
            onResultCallback?.Invoke(_topLeaderboard.Leaderboard);
        }

        void OnErrorCallback(PlayFabError error)
        {
            onErrorCallback?.Invoke(error.GenerateErrorReport());
        }
    }
}
#endif