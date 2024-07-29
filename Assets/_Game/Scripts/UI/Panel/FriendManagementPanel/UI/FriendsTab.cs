using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FriendsTab : MonoBehaviour
{
    #region UIElements
    [SerializeField] private TMP_Text _displayName;
    [SerializeField] private Image _avatar;
    [SerializeField] private TMP_Text _profileLevel;
    [SerializeField] private TMP_Text _trophyCount;
    #endregion
    private FriendInteractionButtonGroupController _friendInteractionButtonGroupController;
    private FriendInteractionButtonGroupController FriendInteractionButtonGroupController
    {
        get
        {
            if (_friendInteractionButtonGroupController == null)
            {
                _friendInteractionButtonGroupController = GetComponentInChildren<FriendInteractionButtonGroupController>();
            }
            return _friendInteractionButtonGroupController;
        }
    }
    
    private FriendInfoModel _friendInfoModel;
    public void SetData(FriendInfoModel friendInfoModel)
    {
        _friendInfoModel = friendInfoModel;
        var shortUsername = friendInfoModel.DisplayName.ShortUsername;
        _displayName.text = shortUsername =="Guest" ?friendInfoModel.DisplayName.LongUsername: shortUsername;
        _avatar.sprite = AvatarService.GetAvatarById(friendInfoModel.AvatarUrl).avatar;
        _profileLevel.text = friendInfoModel.ProfileSummary.profile.Level.ToString();
        _trophyCount.text = friendInfoModel.ProfileSummary.profile.TrophyCount.ToString();
        FriendInteractionButtonGroupController.SetData(friendInfoModel);
    }
}
