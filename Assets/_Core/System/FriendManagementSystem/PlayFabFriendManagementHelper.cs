#if PlayFabSdk_Enabled

using System;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayFabFriendManagementHelper
{
    #region Friends

    private async void SendFriendRequest(string playFabId,
        Action<FriendRequestResponse> OnSendFriendRequestSuccess = null,
        Action<FriendRequestResponse> OnSendFriendRequestFail = null)
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName =
                    "SendFriendRequest", // Arbitrary function name (must exist in your uploaded cloud.js file)
                FunctionParameter = new { FriendPlayFabId = playFabId }, // The parameter provided to your function
                GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
            },
            OnRequestSuccess, OnRequestFail);

        void OnRequestSuccess(ExecuteCloudScriptResult e)
        {
            FriendRequestResponse frResponse = new FriendRequestResponse();
            frResponse.success = true;
            if (e.Logs.Count > 0)
            {
                var jsonResult = JsonHelper.FromJson(e.Logs[0].Data.ToString());
                JObject jsonObject = (JObject)jsonResult;
                if (!jsonObject.TryGetValue("apiError", out var error)) return;
                if (((JObject)error).TryGetValue("errorCode", out var code))
                {
                    frResponse.code = Convert.ToInt32(code);
                    frResponse.success = false;
                    frResponse.message = frResponse.code != 1000
                        ? PlayFabHelper.ErrorText[(PlayFabErrorCode)frResponse.code]
                        : ((JObject)error)["errorMessage"]?.ToString();
                }
            }

            OnSendFriendRequestSuccess?.Invoke(frResponse);
        }

        void OnRequestFail(PlayFabError error)
        {
            FriendRequestResponse frResponse = new FriendRequestResponse();
            frResponse.code = Convert.ToInt32(error.Error);
            frResponse.success = false;
            frResponse.message = PlayFabHelper.ErrorText[error.Error];
            Debug.LogError(error.GenerateErrorReport());
            OnSendFriendRequestFail?.Invoke(frResponse);
        }
    }

    public async void SendFriendRequest(string input, FriendIdType friendIdType,
        Action<FriendRequestResponse> OnSendFriendRequestSuccess = null,
        Action<FriendRequestResponse> OnSendFriendRequestFail = null)
    {
        switch (friendIdType)
        {
            case FriendIdType.Custom:
                SendFriendRequest(input, OnSendFriendRequestSuccess, OnSendFriendRequestFail);
                break;
            case FriendIdType.DisplayName:
                PlayFabHelper.GetFriendInfoFromDisplayName(input).ContinueWith((friendInfo) =>
                {
                    SendFriendRequest(friendInfo.PlatformId, OnSendFriendRequestSuccess, OnSendFriendRequestFail);
                });
                break;
        }
    }

    public void AcceptFriendRequest(string playFabId,
        Action<FriendRequestResponse> OnAcceptFriendRequestSuccess = null,
        Action<FriendRequestResponse> OnAcceptFriendRequestFail = null)
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName =
                "AcceptFriendRequest", // Arbitrary function name (must exist in your uploaded cloud.js file)
            FunctionParameter = new { FriendPlayFabId = playFabId }, // The parameter provided to your function
            GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
        }, OnRequestSuccess, OnRequestFail);

        void OnRequestSuccess(ExecuteCloudScriptResult e)
        {
            FriendRequestResponse frResponse = new FriendRequestResponse();
            frResponse.success = true;
            if (e.Logs.Count > 0)
            {
                var jsonResult = JsonHelper.FromJson(e.Logs[0].Data.ToString());
                JObject jsonObject = (JObject)jsonResult;
                if (!jsonObject.TryGetValue("apiError", out var error)) return;
                if (((JObject)error).TryGetValue("errorCode", out var code))
                {
                    frResponse.code = Convert.ToInt32(code);
                    frResponse.success = false;
                    frResponse.message = PlayFabHelper.ErrorText[(PlayFabErrorCode)frResponse.code];
                }
            }

            OnAcceptFriendRequestSuccess?.Invoke(frResponse);
        }

        void OnRequestFail(PlayFabError error)
        {
            FriendRequestResponse frResponse = new FriendRequestResponse();
            frResponse.code = Convert.ToInt32(error.Error);
            frResponse.success = false;
            frResponse.message = PlayFabHelper.ErrorText[error.Error];
            Debug.LogError(error.GenerateErrorReport());
            OnAcceptFriendRequestFail?.Invoke(frResponse);
        }
    }

    public void DenyFriendRequest(string playFabId,
        Action<FriendRequestResponse> OnDenyFriendRequestSuccess = null,
        Action<FriendRequestResponse> OnDenyFriendRequestFail = null)
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "DenyFriendRequest", // Arbitrary function name (must exist in your uploaded cloud.js file)
            FunctionParameter = new { FriendPlayFabId = playFabId }, // The parameter provided to your function
            GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
        }, OnRequestSuccess, OnRequestFail);

        void OnRequestSuccess(ExecuteCloudScriptResult e)
        {
            FriendRequestResponse frResponse = new FriendRequestResponse();
            frResponse.success = true;
            if (e.Logs.Count > 0)
            {
                var jsonResult = JsonHelper.FromJson(e.Logs[0].Data.ToString());
                JObject jsonObject = (JObject)jsonResult;
                if (!jsonObject.TryGetValue("apiError", out var error)) return;
                if (((JObject)error).TryGetValue("errorCode", out var code))
                {
                    frResponse.code = Convert.ToInt32(code);
                    frResponse.success = false;
                    frResponse.message = PlayFabHelper.ErrorText[(PlayFabErrorCode)frResponse.code];
                }
            }

            OnDenyFriendRequestSuccess?.Invoke(frResponse);
        }

        void OnRequestFail(PlayFabError error)
        {
            FriendRequestResponse frResponse = new FriendRequestResponse();
            frResponse.code = Convert.ToInt32(error.Error);
            frResponse.success = false;
            frResponse.message = PlayFabHelper.ErrorText[error.Error];
            Debug.LogError(error.GenerateErrorReport());
            OnDenyFriendRequestFail?.Invoke(frResponse);
        }
    }
    #endregion
}
#endif
