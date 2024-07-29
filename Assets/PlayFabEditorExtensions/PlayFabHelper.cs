#if PlayFabSdk_Enabled
using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
// using Hellmade.Net;
using Newtonsoft.Json.Linq;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public static class PlayFabHelper
{
    private static DataManager _dataManager = GameInstaller.Instance.SystemLocator.DataManager;

    #region Login Properties

    // Flag set after successfull Playfab Login
    public static bool IsLoggedIn { get; set; } = false;

    public static string UserId { get; set; }
    public static string PlayFabId { get; set; }
    public static string CountryCode { get; set; }

    #endregion Login Properties

    #region Error Codes
    
    public static readonly Dictionary<PlayFabErrorCode, string> ErrorText = new()
    {
        { PlayFabErrorCode.AccountBanned, "Account banned!" },
        { PlayFabErrorCode.AccountNotFound, "Account not found" },
        { PlayFabErrorCode.InvalidParams, "Invalid parameters" },
        { PlayFabErrorCode.InvalidUsernameOrPassword, "Invalid username or password" },
        { PlayFabErrorCode.NameNotAvailable, "Name not available" },
        { PlayFabErrorCode.UserAlreadyAdded, "User already added" },
        { PlayFabErrorCode.UsersAlreadyFriends, "Users already friends" }
    };
    
    #endregion Error Codes

    #region Update Username and Avatar

    public static async UniTask UpdateUsername(string username)
    {
        if (!PlayFabClientAPI.IsClientLoggedIn())
            return;

        bool responseReceived = false;

        var request = new UpdateUserTitleDisplayNameRequest { DisplayName = username };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnSuccess, OnError);

        void OnSuccess(UpdateUserTitleDisplayNameResult response)
        {
            Debug.Log("The player's display name is now: " + response.DisplayName);

            // Update the username in the local storage (persistent data).
            var profileState = _dataManager.GetData<ProfileModel>(GameDataType.ProfileData);
            profileState.Name.Parse(response.DisplayName);
            _dataManager.SetData(GameDataType.ProfileData, profileState);
            var profileSummary = _dataManager.GetData<ProfileSummaryData>(GameDataType.ProfileSummaryData);
            profileSummary.profile.Name.Parse(response.DisplayName);
            _dataManager.SetData(GameDataType.ProfileSummaryData, profileSummary);
            _dataManager.SaveData();

            responseReceived = true;
        }

        void OnError(PlayFabError error)
        {
            Debug.LogError(error.GenerateErrorReport());
            responseReceived = true;
        }

        await UniTask.WaitUntil(() => responseReceived);
    }

    public static async UniTask UpdateAvatar(int avatarIndex)
    {
        if (!PlayFabClientAPI.IsClientLoggedIn())
            return;

        bool responseReceived = false;

        var request = new UpdateAvatarUrlRequest() { ImageUrl = avatarIndex.ToString() };
        PlayFabClientAPI.UpdateAvatarUrl(request, OnSuccess, OnError);

        void OnSuccess(EmptyResponse response)
        {
            Debug.Log("The player's avatar url is now: " + avatarIndex);

            // Update the username in the local storage (persistent data).
            var profileData = _dataManager.GetData<ProfileModel>(GameDataType.ProfileData);
            profileData.AvatarIndex = avatarIndex.ToString();
            _dataManager.SetData(GameDataType.ProfileData, profileData);
            _dataManager.SaveData();
            responseReceived = true;
        }

        void OnError(PlayFabError error)
        {
            Debug.LogError(error.GenerateErrorReport());
            responseReceived = true;
        }

        await UniTask.WaitUntil(() => responseReceived);
    }

    #endregion Update Username and Avatar

    #region Update Statistics

    public static void UpdateStatistics(Dictionary<string, object> values)
    {
        if (!PlayFabClientAPI.IsClientLoggedIn())
            return;
        // var internetStatus = EazyNetChecker.Status;
        // if (internetStatus == NetStatus.NoDNSConnection) return;
        var stats = values.Select(keyValuePair => new StatisticUpdate
            { StatisticName = keyValuePair.Key, Value = Convert.ToInt32(keyValuePair.Value) }).ToList();
        PlayFabClientAPI.UpdatePlayerStatistics(
            // Request
            new UpdatePlayerStatisticsRequest
            {
                Statistics = stats
            },
            // Success
            result => { Debug.Log("User statistics updated"); },
            // Failure
            (PlayFabError error) =>
            {
                Debug.LogError(error);
                Debug.LogError(error.GenerateErrorReport());
            }
        );
    }

    public static void UpdateStatistic(string stat, int value)
    {
        if (!PlayFabClientAPI.IsClientLoggedIn())
            return;

        PlayFabClientAPI.UpdatePlayerStatistics(
            // Request
            new UpdatePlayerStatisticsRequest
            {
                Statistics = new List<StatisticUpdate>
                {
                    new StatisticUpdate
                    {
                        StatisticName = stat,
                        Value = value
                    }
                }
            },
            // Success
            result =>
            {
                //Debug.Log("User statistics updated");
            },
            // Failure
            (PlayFabError error) =>
            {
                Debug.LogError(error);
                Debug.LogError(error.GenerateErrorReport());
            }
        );
    }

    #endregion Update Statistics

    #region Profile
    public static async UniTask<ProfileSummaryData> GetProfileSummary(string playFabId)
    {
        ProfileSummaryData profileSummaryData = new ProfileSummaryData();
        bool isReceived = false;
        
        GetUserDataRequest request = new GetUserDataRequest()
        {
            PlayFabId = playFabId,
            Keys = new List<string>(){GameData.GetGameDataNameOf(GameDataType.ProfileSummaryData)},
        };

        PlayFabClientAPI.GetUserData(request,
result => {
                if (result.Data.TryGetValue(GameData.GetGameDataNameOf(GameDataType.ProfileSummaryData), out var data))
                {
                    profileSummaryData = JsonHelper.FromJson<ProfileSummaryData>(data.Value);
                }
                isReceived = true;
            }, error => {
                Debug.LogError(error.GenerateErrorReport());
                isReceived = true;
            });

        await UniTask.WaitUntil(()=> isReceived); 
        return profileSummaryData;
    }
    public static async UniTask<FriendInfoModel> GetFriendInfoFromDisplayName(string displayName)
    {
        FriendInfoModel friendInfoModel = null;
        bool finished = false;
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest()
            {
                TitleDisplayName = displayName,
            },
            result =>
            {
                finished = true;
                friendInfoModel = new FriendInfoModel
                {
                    DisplayName = new UsernameModel(result.AccountInfo.TitleInfo.DisplayName),
                    PlatformId = result.AccountInfo.PlayFabId,
                    AvatarUrl = result.AccountInfo.TitleInfo.AvatarUrl,
                    FriendshipStatus = FriendshipStatus.None
                };
            },
            error => { finished = true; });
        await UniTask.WaitUntil(() => finished).Timeout(TimeSpan.FromSeconds(5));
        return friendInfoModel;
    }
    #endregion
    public static RequestResponse ResponseHandling(ExecuteCloudScriptResult e)
    {
        RequestResponse response = new RequestResponse();
        response.Success = true;
        if (e.Logs.Count > 0)
        {
            var jsonResult = JsonHelper.FromJson(e.Logs[0].Data.ToString());
            JObject jsonObject = (JObject)jsonResult;
            if (!jsonObject.TryGetValue("apiError", out var error)) return response;
            if (((JObject)error).TryGetValue("errorCode", out var code))
            {
                response.Code = Convert.ToInt32(code);
                response.Success = false;
                response.Message = ErrorText[(PlayFabErrorCode)response.Code];
            }
        }
        else
        {
            response = JsonHelper.FromJson<RequestResponse>(e.FunctionResult.ToString());
        }

        return response;
    }

    public static RequestResponse ResponseHandling(PlayFabError error)
    {
        RequestResponse response = new RequestResponse();
        response.Code = Convert.ToInt32(error.Error);
        response.Success = false;
        response.Message = ErrorText[error.Error];
        Debug.LogError(error.GenerateErrorReport());
        return response;
    }
}
#endif
public class RequestResponse
{
    public int Code { get; set; }
    public string Message { get; set; }
    public bool Success { get; set; }
    public object Data { get; set; }
}