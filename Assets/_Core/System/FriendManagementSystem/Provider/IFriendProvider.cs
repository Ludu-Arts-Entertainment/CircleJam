using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

public interface IFriendProvider
{
    IFriendProvider CreateSelf();
    void Initialize(Action onReady);
    bool IsAvailable { get; }
    delegate void FriendListUpdated();

    FriendListUpdated OnFriendListUpdated { get; set; }

    UniTask SendFriendRequest(FriendRequestArguments arguments,
        Action<FriendRequestResponse> OnSendFriendRequestSuccess = null,
        Action<FriendRequestResponse> OnSendFriendRequestFail = null);

    UniTask AcceptFriendRequest(FriendRequestArguments arguments,
        Action<FriendRequestResponse> OnAcceptFriendRequestSuccess = null,
        Action<FriendRequestResponse> OnAcceptFriendRequestFail = null);

    UniTask DenyFriendRequest(FriendRequestArguments arguments,
        Action<FriendRequestResponse> OnDenyFriendRequestSuccess = null,
        Action<FriendRequestResponse> OnDenyFriendRequestFail = null);

    UniTask<List<FriendInfoModel>> GetFriendsList(List<FriendshipStatus> status = null, float updateInterval = default ,bool forceUpdate = false);
}

public class FriendRequestResponse
{
    public int code { get; set; }
    public string message { get; set; }
    public bool success { get; set; }
}