using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendManagementPopup : PopupBase
{
    
    [BoxGroup("UI Elements")]
    [SerializeField] private Image _avatar;
    
    [BoxGroup("UI Elements")]
    [SerializeField] private DisplayNameTextController _displayNameText;
    
    [BoxGroup("UI Elements")]
    [SerializeField] private FriendInteractionButtonGroupController friendInteractionButtonGroupController;
    
    private FriendInfoModel _friendInfoModel { get; set; }
    
    #region Managers
    private FriendManager _friendManager;
    #endregion
    public override void Show(IBaseUIData data)
    {
        base.InputBlocker(true, Close);
        _friendInfoModel = (data as FriendManagementPopupData)?.FriendInfoModel;
        SetData();
        _friendManager??=GameInstaller.Instance.SystemLocator.FriendManager;
        _friendManager.OnFriendListUpdated += OnFriendListUpdated;

        base.Show(data);
    }
    
    public override void Hide()
    {
        base.InputBlocker(false,Close);
        _friendManager.OnFriendListUpdated -= OnFriendListUpdated;
        base.Hide();
    }
    private void OnFriendListUpdated()
    {
        if (_friendInfoModel != null)
        {
            _friendManager.GetFriendsList().ContinueWith(friends =>
            {
                var friendInfoModel = friends.Find(x => x.PlatformId == _friendInfoModel.PlatformId);
                if (friendInfoModel != null)
                {
                    _friendInfoModel = friendInfoModel;
                    SetData();
                }
                else
                {
                    _friendInfoModel.FriendshipStatus = FriendshipStatus.None;
                    SetData();
                }
            });
        }
    }

    private void SetData()
    {
        SetAvatar(Resources.Load<Sprite>("Avatars/"+_friendInfoModel.AvatarUrl));
        SetDisplayName();
        friendInteractionButtonGroupController.SetData(_friendInfoModel);
        
    }
    private void SetAvatar(Sprite avatar)
    {
        _avatar.sprite = avatar;
    }
    private void SetDisplayName()
    {
        _displayNameText.SetText(_friendInfoModel.DisplayName);
    }
    private void SetButtons()
    {
        
    }
    public void Close()
    {
        GameInstaller.Instance.SystemLocator.UIManager.Hide(UITypes.FriendManagementPopup);
    }
}

public class FriendManagementPopupData : BaseUIData
{
    public FriendInfoModel FriendInfoModel { get; set; }

    public FriendManagementPopupData(FriendInfoModel friendInfoModel)
    {
        FriendInfoModel = friendInfoModel;
    }
}

public partial class UITypes
{
    public const string FriendManagementPopup = "FriendManagementPopup";
}
