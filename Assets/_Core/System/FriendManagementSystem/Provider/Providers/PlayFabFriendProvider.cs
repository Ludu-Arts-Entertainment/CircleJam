#if PlayFabSdk_Enabled

using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayFabFriendProvider : IFriendProvider
{
    private Dictionary<string, FriendInfoModel> _friendInfoDict;
    private float _lastUpdateTime;
    private const float UpdateInterval = 15f;
    private PlayFabFriendManagementHelper _playFabFriendManagementHelper;

    public IFriendProvider CreateSelf()
    {
        return new PlayFabFriendProvider();
    }

    public void Initialize(Action onReady)
    {
        onReady?.Invoke();
        _playFabFriendManagementHelper = new PlayFabFriendManagementHelper();
        UniTask.WaitUntil(PlayFabClientAPI.IsClientLoggedIn).ContinueWith(() =>
        {
            if (ProcessDeepLinkManager.Instance.QueryParameters is {Count:>0})
            {
                OnDeepLinkActivate();
            }
            ProcessDeepLinkManager.Instance.OnDeepLinkActivate += parameters =>
            {
                OnDeepLinkActivate();
            };
        });
        void OnDeepLinkActivate()
        {
            if (!ProcessDeepLinkManager.Instance.QueryParameters.TryGetValue("friendId", out var friendId)) return;
            var usernameModel = new UsernameModel(friendId);
            SendFriendRequest(new FriendRequestArguments()
            {
                FriendId = usernameModel.ShortUsername+usernameModel.UniqueNumber,
                FriendIdType = FriendIdType.DisplayName
            }, results =>
            {
                GameInstaller.Instance.SystemLocator.EventManager.Trigger(new Events.OnFloatingTextAnimationStart(FloatingTextType.MoveUpAndHide ,"Friend Request Sent"));
            }, results =>
            {
                GameInstaller.Instance.SystemLocator.EventManager.Trigger(new Events.OnFloatingTextAnimationStart(FloatingTextType.MoveUpAndHide ,results.message));
            });
        }
    }

    public bool IsAvailable => PlayFabClientAPI.IsClientLoggedIn();

    public IFriendProvider.FriendListUpdated OnFriendListUpdated { get; set; }

    public async UniTask SendFriendRequest(FriendRequestArguments arguments, Action<FriendRequestResponse> OnSendFriendRequestSuccess = null,
        Action<FriendRequestResponse> OnSendFriendRequestFail = null)
    {
        OnSendFriendRequestSuccess+= (results) =>
        {
            if (!results.success)return;
            arguments.FriendInfoModel.FriendshipStatus = FriendshipStatus.Receiver;
            _friendInfoDict ??= new Dictionary<string, FriendInfoModel>();
            _friendInfoDict.Add(arguments.FriendId, arguments.FriendInfoModel);
            _lastUpdateTime-=UpdateInterval;
            OnFriendListUpdated?.Invoke();
        };
        _playFabFriendManagementHelper.SendFriendRequest(arguments.FriendId, arguments.FriendIdType, OnSendFriendRequestSuccess, OnSendFriendRequestFail);
        await UniTask.Yield();
    }

    public async UniTask AcceptFriendRequest(FriendRequestArguments arguments, Action<FriendRequestResponse> OnAcceptFriendRequestSuccess = null,
        Action<FriendRequestResponse> OnAcceptFriendRequestFail = null)
    {
        OnAcceptFriendRequestSuccess+= (results) =>
        {
            if (results.success)
            {
                _friendInfoDict[arguments.FriendId].FriendshipStatus = FriendshipStatus.Confirmed;
            }
            OnFriendListUpdated?.Invoke();
        };
        _playFabFriendManagementHelper.AcceptFriendRequest(arguments.FriendId, OnAcceptFriendRequestSuccess, OnAcceptFriendRequestFail);
        await UniTask.Yield();
    }

    public async UniTask DenyFriendRequest(FriendRequestArguments arguments, Action<FriendRequestResponse> OnDenyFriendRequestSuccess = null,
        Action<FriendRequestResponse> OnDenyFriendRequestFail = null)
    {
        OnDenyFriendRequestSuccess+= (results) =>
        {
            if (!results.success) return;
            _friendInfoDict.Remove(arguments.FriendId);
            OnFriendListUpdated?.Invoke();
        };
        _playFabFriendManagementHelper.DenyFriendRequest(arguments.FriendId, OnDenyFriendRequestSuccess, OnDenyFriendRequestFail);
        await UniTask.Yield();
    }

    public async UniTask<List<FriendInfoModel>> GetFriendsList(List<FriendshipStatus> status = null, float updateInterval = default ,bool forceUpdate = false)
    {
        var request = new GetFriendsListRequest()
        {
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true,
                ShowAvatarUrl = true,
                ShowStatistics = true
            }
        };
        bool finished = false;
        if (_friendInfoDict == null || forceUpdate || _lastUpdateTime + (updateInterval != default?updateInterval:UpdateInterval) <= Time.time)
        {
            PlayFabClientAPI.GetFriendsList(request, GetFriendsListSuccessCallback, GetFriendsListFailCallback);
            void GetFriendsListSuccessCallback(GetFriendsListResult result)
            {
                _lastUpdateTime = Time.time;
                _friendInfoDict ??= new Dictionary<string, FriendInfoModel>();
                _friendInfoDict.Clear();
                result.Friends.ForEach(x =>
                {
                    x.Tags ??= new List<string> { FriendshipStatus.Facebook.ToString() };
                    _friendInfoDict.Add(x.FriendPlayFabId, new FriendInfoModel()
                    {
                        DisplayName = new UsernameModel(x.Profile.DisplayName),
                        PlatformId = x.FriendPlayFabId,
                        AvatarUrl = x.Profile.AvatarUrl,
                        ProfileSummary = new ProfileSummaryData()
                        {
                            profile = new ProfileModel()
                            {
                                Name = new UsernameModel(x.Profile.DisplayName),
                                AvatarIndex = x.Profile.AvatarUrl,
                                Level = x.Profile.Statistics.FirstOrDefault(x => x.Name == "level")?.Value ?? 0,
                                TrophyCount = x.Profile.Statistics.FirstOrDefault(x => x.Name == "trophy_world")?.Value ?? 0
                            }
                        },
                        FriendshipStatus = (FriendshipStatus)Enum.Parse(typeof(FriendshipStatus), x.Tags.FirstOrDefault() ?? string.Empty)
                    });
                });
                OnFriendListUpdated?.Invoke();
                finished = true;
            }
            void GetFriendsListFailCallback(PlayFabError error)
            {
                Debug.Log(error.GenerateErrorReport());
                finished = true;
            }
        }else finished = true;
        
        await UniTask.WaitUntil(() => finished).Timeout(TimeSpan.FromSeconds(5));
        if (status is {Count:0} or null) return _friendInfoDict.Values.ToList();
        return _friendInfoDict.Where(x => status.Contains(x.Value.FriendshipStatus)
            ).Select(x=> x.Value).ToList();
    }
}

#endif