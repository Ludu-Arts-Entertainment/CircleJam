#if MailManager_Enabled

using System.Linq;
using UnityEngine;

public class SendGiftToFriendInteractionButton : FriendInteractionButtonBase
{
    #region Managers
    private MailManager _mailManager;
    #endregion
    private DailyGiftConfig _dailyGiftConfig;
    public override async void SetData(FriendInfoModel friendInfoModel)
    {
        base.SetData(friendInfoModel);
        _dailyGiftConfig ??= GameInstaller.Instance.SystemLocator.RemoteConfigManager.GetObject<MailConfig>().DailyGiftConfig;
        if (_mailManager !=null)
        {
            _mailManager.OnSentMailListUpdated -= UpdateButtonState;
        }else _mailManager = GameInstaller.Instance.SystemLocator.MailManager;
        _mailManager.OnSentMailListUpdated += UpdateButtonState;
        Button.interactable = false;
        UpdateButtonState();
    }
    private async void UpdateButtonState()
    {
        var sentMails = await _mailManager.GetSentMails();
        var sentGiftMails = sentMails.Where(x => x.Tag == MailTag.DailyGift);
        Button.interactable = (_dailyGiftConfig.Amount == -1 || sentGiftMails.Count() < _dailyGiftConfig.Amount) &&
                              !sentGiftMails.Any(x => x.ReceiverId == _friendInfoModel.PlatformId && x.Tag == MailTag.DailyGift);
    }
    public override void Process()
    {
        // if (!isActive)
        // {
        //     GameInstaller.Instance.EventManager.Trigger(
        //         new Events.OnFloatingTextAnimationStart(FloatingTextType.MoveUpAndHide,
        //             "You have already sent gift to this friend\nPlease wait for the next day"));
        //     return;
        // }
        Button.interactable = false;
        _mailManager.SendMail(new MailModel()
        {
            ReceiverId = _friendInfoModel.PlatformId,
#if PlayFabSdk_Enabled
            SenderId = PlayFabHelper.PlayFabId,
#else
            SenderId = "PlayFabSdk not enabled",
#endif
            Tag = MailTag.DailyGift,
            SentTimestamp = (long)TimeHelper.DateTimeToUnixTimeStampInSeconds(TimeHelper.GetCurrentDateTime()),
            ExpireTimestamp = (long)TimeHelper.DateTimeToUnixTimeStampInSeconds(TimeHelper.GetCurrentDateTime())+ _dailyGiftConfig.ExpireTime,
            IsClaimAndDelete = true,
            Status = MailStatus.Unread,
            AttachmentModel = new MailAttachmentModel(){ Products = _dailyGiftConfig.Products},
            Title = string.Format(_dailyGiftConfig.Title, GameInstaller.Instance.SystemLocator.DataManager.GetData<ProfileModel>(GameDataType.ProfileData).Name.ShortUsername),
            Message = _dailyGiftConfig.Message
        }, (response) =>
        {
            Debug.Log(response.Message);
        }, (response) =>
        {
            Debug.Log(response.Message);
        });
    }
}
#endif