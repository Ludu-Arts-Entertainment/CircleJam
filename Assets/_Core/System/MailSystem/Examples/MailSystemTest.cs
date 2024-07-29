using NaughtyAttributes;
using UnityEngine;

public class MailSystemTest : MonoBehaviour
{
    [SerializeField] private string _receiverId;
    [SerializeField] private string _senderId;
    [SerializeField] private string _mailId;
    [SerializeField] private ProductBlock[] _products;

    [Button]
    public void SendMail()
    {
        GameInstaller.Instance.SystemLocator.MailManager.SendMail(new MailModel()
        {
            ReceiverId = _receiverId,
            Title = "Test Mail Title",
#if PlayFabSdk_Enabled
            SenderId = PlayFabHelper.PlayFabId,
#else
            SenderId = "Test Sender Id",
#endif
            Message = "Test Mail Message",
            SentTimestamp = (long)TimeHelper.DateTimeToUnixTimeStampInSeconds(TimeHelper.GetCurrentDateTime()),
            ExpireTimestamp = -1,
            Tag = MailTag.DailyGift,
            Status = MailStatus.Unread,
            AttachmentModel = new MailAttachmentModel() { Products = _products }
        }, onSuccess, onError);

        void onSuccess(RequestResponse requestResponse)
        {
            Debug.Log("Mail Sent");
        }

        void onError(RequestResponse obj)
        {
            Debug.LogError("Mail Send Error");
        }
    }

    [Button]
    public void DeleteMail()
    {
        GameInstaller.Instance.SystemLocator.MailManager.DeleteMail(_senderId, _mailId, onSuccess, onError);

        void onSuccess(RequestResponse requestResponse)
        {
            Debug.Log("Mail Deleted");
        }

        void onError(RequestResponse obj)
        {
            Debug.LogError("Mail Delete Error");
        }
    }

    [Button]
    public void ReadMail()
    {
        GameInstaller.Instance.SystemLocator.MailManager.ReadMail(_senderId, _mailId, onSuccess, onError);

        void onSuccess(RequestResponse requestResponse)
        {
            Debug.Log(requestResponse.Message);
        }

        void onError(RequestResponse obj)
        {
            Debug.Log(obj.Message);
        }
    }

    [Button]
    public void ClaimMail()
    {
        GameInstaller.Instance.SystemLocator.MailManager.ClaimMail(_senderId, _mailId, onSuccess, onError);

        void onSuccess(RequestResponse requestResponse)
        {
            Debug.Log(requestResponse.Message);
        }

        void onError(RequestResponse obj)
        {
            Debug.Log(obj.Message);
        }
    }

    [Button]
    public void GetSentMails()
    {
        GameInstaller.Instance.SystemLocator.MailManager.GetSentMails();
    }

    [Button]
    public async void GetWaitMails()
    {
        var mails = await GameInstaller.Instance.SystemLocator.MailManager.GetWaitMails();
        Debug.Log(string.Join("\n", mails.ConvertAll(x => x.Title).ToArray()));
    }
}