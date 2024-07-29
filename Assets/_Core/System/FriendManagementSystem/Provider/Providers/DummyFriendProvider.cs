using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class DummyFriendProvider : IFriendProvider
{
    public IFriendProvider CreateSelf()
    {
        return new DummyFriendProvider();
    }
    public void Initialize(Action onReady)
    {
        onReady?.Invoke();
    }

    public bool IsAvailable => true;

    public IFriendProvider.FriendListUpdated OnFriendListUpdated { get; set; }

    public async UniTask SendFriendRequest(FriendRequestArguments arguments, Action<FriendRequestResponse> OnSendFriendRequestSuccess = null, Action<FriendRequestResponse> OnSendFriendRequestFail = null)
    {
        Debug.Log($"{nameof(DummyFriendProvider)}.{nameof(SendFriendRequest)} was called");
        OnSendFriendRequestSuccess?.Invoke(new FriendRequestResponse());
        OnFriendListUpdated?.Invoke();
        await UniTask.Yield();
    }

    public async UniTask AcceptFriendRequest(FriendRequestArguments arguments, Action<FriendRequestResponse> OnAcceptFriendRequestSuccess = null, Action<FriendRequestResponse> OnAcceptFriendRequestFail = null)
    {
        Debug.Log($"{nameof(DummyFriendProvider)}.{nameof(AcceptFriendRequest)} was called");
        OnAcceptFriendRequestSuccess?.Invoke(new FriendRequestResponse());
        OnFriendListUpdated?.Invoke();

        await UniTask.Yield();
    }

    public async UniTask DenyFriendRequest(FriendRequestArguments arguments, Action<FriendRequestResponse> OnDenyFriendRequestSuccess = null, Action<FriendRequestResponse> OnDenyFriendRequestFail = null)
    {
        Debug.Log($"{nameof(DummyFriendProvider)}.{nameof(DenyFriendRequest)} was called");
        OnDenyFriendRequestSuccess?.Invoke(new FriendRequestResponse());
        OnFriendListUpdated?.Invoke();
        await UniTask.Yield();
    }

    public async UniTask<List<FriendInfoModel>> GetFriendsList(List<FriendshipStatus> status = null, float updateInterval = default ,bool forceUpdate = false)
    {
        Debug.Log($"{nameof(DummyFriendProvider)}.{nameof(GetFriendsList)} was called");
        await UniTask.Yield();
        return new List<FriendInfoModel>();
    }
}