using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class FriendSystemTest : MonoBehaviour
{
    public string PlayFabId;
    public FriendshipStatus friendshipStatus = FriendshipStatus.None;

    [Button("SendFriendRequest")]
    public void SendFriendRequest()
    {
        GameInstaller.Instance.SystemLocator.FriendManager.SendFriendRequest(new FriendRequestArguments()
        {
            FriendId = PlayFabId,
            FriendIdType = FriendIdType.Custom
        });
    }

    [Button("AcceptFriendRequest")]
    public void AcceptFriendRequest()
    {
        GameInstaller.Instance.SystemLocator.FriendManager.AcceptFriendRequest(new FriendRequestArguments()
        {
            FriendId = PlayFabId,
            FriendIdType = FriendIdType.Custom
        });
    }

    [Button("DenyFriendRequest")]
    public void DenyFriendRequest()
    {
        GameInstaller.Instance.SystemLocator.FriendManager.DenyFriendRequest(new FriendRequestArguments()
        {
            FriendId = PlayFabId,
            FriendIdType = FriendIdType.Custom
        });
    }

    [Button("GetFriendsList")]
    public void GetFriendsList()
    {
        GameInstaller.Instance.SystemLocator.FriendManager.GetFriendsList(new List<FriendshipStatus>(){friendshipStatus}).ContinueWith(friends =>
        {
            foreach (var friend in friends)
            {
                Debug.Log($"Friend: {friend.DisplayName} - {friend.PlatformId}");
            }
        });
    }

    [Button("GetPlayerWithDisplayName")]
    public void GetPlayerWithDisplayName()
    {
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest()
            {
                TitleDisplayName = PlayFabId
            },
            result =>
            {
                Debug.Log($"GetPlayerWithDisplayName: {result.AccountInfo.PlayFabId}"); 
                
            },
            error =>
            {
                Debug.LogError(error.GenerateErrorReport()); 
            });
    }

    [Button("Open Panel")]
    public void OpenFriendPanel()
    {
        //GameInstaller.Instance.SystemLocator.UIManager.Show(UITypes.FriendManagementPanel,null);
    }
}