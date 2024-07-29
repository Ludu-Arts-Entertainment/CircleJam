using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class FriendManager : IManager
{
    private IFriendProvider friendProvider;
    public IFriendProvider.FriendListUpdated OnFriendListUpdated
    {
        get => friendProvider.OnFriendListUpdated;
        set => friendProvider.OnFriendListUpdated = value;
    }
    public IManager CreateSelf()
    {
        return new FriendManager();
    }

    public void Initialize(GameInstaller gameInstaller, Action onReady)
    {
        friendProvider = FriendProviderFactory.Create(GameInstaller.Instance.Customizer.FriendProvider);
        friendProvider.Initialize(onReady);
    }

    public bool IsReady()
    {
        return friendProvider != null;
    }
    public UniTask SendFriendRequest(FriendRequestArguments arguments, Action<FriendRequestResponse> OnSendFriendRequestSuccess = null,
        Action<FriendRequestResponse> OnSendFriendRequestFail = null)
    {
        if (!friendProvider.IsAvailable) return default;
        return friendProvider.SendFriendRequest(arguments, OnSendFriendRequestSuccess, OnSendFriendRequestFail);
    }
    public UniTask AcceptFriendRequest(FriendRequestArguments arguments, Action<FriendRequestResponse> OnAcceptFriendRequestSuccess = null,
        Action<FriendRequestResponse> OnAcceptFriendRequestFail = null)
    {
        if (!friendProvider.IsAvailable) return default;

        return friendProvider.AcceptFriendRequest(arguments, OnAcceptFriendRequestSuccess, OnAcceptFriendRequestFail);
    }
    public UniTask DenyFriendRequest(FriendRequestArguments arguments, Action<FriendRequestResponse> OnDenyFriendRequestSuccess = null,
        Action<FriendRequestResponse> OnDenyFriendRequestFail = null)
    {
        if (!friendProvider.IsAvailable) return default;

        return friendProvider.DenyFriendRequest(arguments, OnDenyFriendRequestSuccess, OnDenyFriendRequestFail);
    }
    public UniTask<List<FriendInfoModel>> GetFriendsList(List<FriendshipStatus> status = null, float updateInterval = default ,bool forceUpdate = false)
    {
        if (!friendProvider.IsAvailable) return default;

        return friendProvider.GetFriendsList(status);
    }
    public UniTask RemoveFriend(FriendRequestArguments arguments)
    {
        if (!friendProvider.IsAvailable) return default;

        return friendProvider.DenyFriendRequest(arguments);
    }
    public UniTask CancelFriendRequest(FriendRequestArguments arguments)
    {
        if (!friendProvider.IsAvailable) return default;

        return friendProvider.DenyFriendRequest(arguments);
    }
}
public enum FriendshipStatus
{
    None=0,
    Sender = 1,
    Receiver = 2,
    Confirmed = 3,
    Facebook = 4,
    Self = 5
}