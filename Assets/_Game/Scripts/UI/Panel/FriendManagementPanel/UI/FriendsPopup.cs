using UnityEngine;
using UnityEngine.UI;

public class FriendsPopup : PopupBase
{
    #region TabToggles
    [SerializeField] private FriendsTabToggle _friendsToggle;
    [SerializeField] private FriendsTabToggle _inviteToggle;
    #endregion
    #region Contents
    [SerializeField] private RectTransform _friendsContent;
    [SerializeField] private RectTransform _inviteContent;
    #endregion

    #region Components
    [SerializeField] private DisplayNameTextController _displayNameTextController; 
    #endregion
    public override void Show(IBaseUIData data)
    {
        _friendsToggle.OnValueChangedAction += OnFriendsTabValueChanged;
        _inviteToggle.OnValueChangedAction += OnInviteTabValueChanged;
        _friendsToggle.isOn = true;
        _displayNameTextController.SetText(GameInstaller.Instance.SystemLocator.DataManager.GetData<ProfileModel>(GameDataType.ProfileData).Name, "Your ID: {0}");
        base.Show(data);
        base.InputBlocker(true);
    }
    public override void Hide()
    {
        _friendsToggle.OnValueChangedAction -= OnFriendsTabValueChanged;
        _inviteToggle.OnValueChangedAction -= OnInviteTabValueChanged;
        base.Hide();
        base.InputBlocker(false);
    }
    private void OnInviteTabValueChanged(bool arg0)
    {
        _inviteContent.gameObject.SetActive(arg0);
    }
    private void OnFriendsTabValueChanged(bool arg0)
    {
        _friendsContent.gameObject.SetActive(arg0);
    }
    public void Close()
    {
        GameInstaller.Instance.SystemLocator.UIManager.Hide(UITypes.FriendsPopup);
    }
}
public partial class UITypes
{
    public const string FriendsPopup = nameof(FriendsPopup);
}