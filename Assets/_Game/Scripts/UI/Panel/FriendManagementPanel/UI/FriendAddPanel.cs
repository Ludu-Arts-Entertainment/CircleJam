using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendAddPanel : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private Button _inviteFriendButton;
    [SerializeField] private Button _searchButton;
    [SerializeField] private Button _clearButton;
    
    private UsernameModel _invitingFriendName;
    private Tween _searchResultPanelTween;
    // private NativeShare _nativeShare;
    
    #region Managers
    private DataManager _dataManager;
    private EventManager _eventManager;
    private FriendManager _friendManager;
    #endregion

    private void Awake()
    {
        _dataManager = GameInstaller.Instance.SystemLocator.DataManager;
        _eventManager = GameInstaller.Instance.SystemLocator.EventManager;
        _friendManager = GameInstaller.Instance.SystemLocator.FriendManager;
    }
    private void OnEnable()
    {
        SetInputField("");
        _inviteFriendButton.onClick.AddListener(OnInviteButtonClicked);
        _inputField.onValueChanged.AddListener(x =>
        {
            _searchButton.interactable = !string.IsNullOrEmpty(x);
            _clearButton?.gameObject.SetActive(!string.IsNullOrEmpty(x));
        });
    }
    private void OnDisable()
    {
        _inviteFriendButton.onClick.RemoveListener(OnInviteButtonClicked);
        _inputField.onValueChanged.RemoveListener(x =>
        {
            _searchButton.interactable = !string.IsNullOrEmpty(x);
            _clearButton?.gameObject.SetActive(!string.IsNullOrEmpty(x));
        });
    }
    
    public void SetInputField(string friendId)
    {
        _inputField.text = friendId;
    }

    public void SearchFriend()
    {
        _searchButton.interactable = false;
        UniTask.Delay(1000).ContinueWith(()=>_searchButton.interactable = true);
        _invitingFriendName  = new UsernameModel(_inputField.text);
        if (_inputField.text.Length< 8 || _invitingFriendName.UniqueNumber == "000000")
        {
            _eventManager.Trigger(new Events.OnFloatingTextAnimationStart(FloatingTextType.MoveUpAndHide,
                "Invalid friend id"));
            return;
        }

#if PlayFabSdk_Enabled
        var task = PlayFabHelper.GetFriendInfoFromDisplayName(_invitingFriendName.ShortUsername+_invitingFriendName.UniqueNumber);
#else
        var task = UniTask.FromResult(null as FriendInfoModel);
#endif
        task.ContinueWith(OnGettingPlayFabId);
        var _friendInfoModel = null as FriendInfoModel;
        void OnGettingPlayFabId(FriendInfoModel friendInfoModel)
        {
            if (friendInfoModel != null)
            {
                _friendInfoModel = friendInfoModel;
                var getFriendRequest = _friendManager.GetFriendsList();
                getFriendRequest.ContinueWith(OnGettingFriendList);
            }
            else
            {
                _eventManager.Trigger(new Events.OnFloatingTextAnimationStart(FloatingTextType.MoveUpAndHide,
                    "Friend not found"));
            }
        }
        void OnGettingFriendList(List<FriendInfoModel> friendInfoModels)
        {
            var index = friendInfoModels.FindIndex(x => x.DisplayName.LongUsername.Equals(_invitingFriendName.LongUsername, StringComparison.OrdinalIgnoreCase));
            if (index==-1)
            {
                _friendManager.SendFriendRequest(new FriendRequestArguments()
                {
                    FriendId = _friendInfoModel.PlatformId,
                    FriendIdType = FriendIdType.Custom,
                    FriendInfoModel = _friendInfoModel
                }, OnRequestSuccess, OnRequestFail);
        
                void OnRequestSuccess(FriendRequestResponse obj)
                {
                    if (obj.success)
                    {
                        _eventManager.Trigger(new Events.OnFloatingTextAnimationStart(FloatingTextType.MoveUpAndHide,
                            "Friend request sent"));
                    }
                    else
                        _eventManager.Trigger(
                            new Events.OnFloatingTextAnimationStart(FloatingTextType.MoveUpAndHide, obj.message));
                }
                void OnRequestFail(FriendRequestResponse obj)
                {
                    _eventManager.Trigger(
                        new Events.OnFloatingTextAnimationStart(FloatingTextType.MoveUpAndHide, obj.message));
                }
            }
            else
            {
                switch (_friendInfoModel.FriendshipStatus)
                {
                    case FriendshipStatus.Self:
                        _eventManager.Trigger(new Events.OnFloatingTextAnimationStart(FloatingTextType.MoveUpAndHide,
                            "You aren't allowed to send friend request to yourself"));
                        break;
                    case FriendshipStatus.Receiver:
                        _eventManager.Trigger(new Events.OnFloatingTextAnimationStart(FloatingTextType.MoveUpAndHide,
                            "Friend request already sent"));
                        break;
                    default:
                        _eventManager.Trigger(new Events.OnFloatingTextAnimationStart(FloatingTextType.MoveUpAndHide,
                            "You are already friends"));
                        break;
                }
            }
        }
    }
    
    private void OnInviteButtonClicked()
    {
        StartCoroutine(ShareLinkRoutine());
    }
    private IEnumerator ShareLinkRoutine()
    {
        yield return new WaitForEndOfFrame();
        yield break;
        // _nativeShare ??= new NativeShare();
        // _nativeShare.Clear();
        // _nativeShare
        //      .SetSubject("Rumble of Spells - Friend Request")
        //      .SetTitle("Rumble of Spells")
        //      .SetText("Click this link to add as friend in Rumble of Spells!")
        //      .SetUrl("https://www.luduarts.com/ros?friendId=" + _dataManager.GetData<ProfileModel>(GameDataType.ProfileData).Name.LongUsername)
        //      .SetCallback((result, shareTarget) =>
        //      {
        //          switch (result)
        //          {
        //              case NativeShare.ShareResult.Shared:
        //                  _eventManager.Trigger(new Events.OnFloatingTextAnimationStart(FloatingTextType.MoveUpAndHide,
        //                      "Share success"));
        //                  break;
        //              case NativeShare.ShareResult.NotShared:
        //                  _eventManager.Trigger(new Events.OnFloatingTextAnimationStart(FloatingTextType.MoveUpAndHide,
        //                      "Share failed"));
        //                  break;
        //              case NativeShare.ShareResult.Unknown:
        //                  _eventManager.Trigger(new Events.OnFloatingTextAnimationStart(FloatingTextType.MoveUpAndHide,
        //                      "Share cancelled"));
        //                  break;
        //          }
        //      })
        //      .Share();

         // Share on WhatsApp only, if installed (Android only)
        // if( NativeShare.TargetExists( "com.whatsapp" ) )
        // 	new NativeShare().AddFile( filePath ).AddTarget( "com.whatsapp" ).Share();
    }
}