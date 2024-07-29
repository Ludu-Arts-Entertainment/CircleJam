using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public interface IMailProvider
{
    IMailProvider CreateSelf();
    delegate void MailListUpdate();
    MailListUpdate OnSentMailListUpdated { get; set; }
    MailListUpdate OnWaitMailListUpdated { get; set; }
    delegate void MailUpdate(string mailId);
    MailUpdate OnMailDeleted { get; set; }
    MailUpdate OnMailRead { get; set; }
    MailUpdate OnMailClaimed { get; set; }
    
    void SendMail(MailModel mailModel, System.Action<RequestResponse> onSuccess, System.Action<RequestResponse> onError);
    void DeleteMail(string senderId, string mailId, System.Action<RequestResponse> onSuccess, System.Action<RequestResponse> onError);
    void ReadMail(string senderId, string mailId, System.Action<RequestResponse> onSuccess, System.Action<RequestResponse> onError);
    void ClaimMail(string senderId, string mailId, System.Action<RequestResponse> onSuccess, System.Action<RequestResponse> onError);
    UniTask<List<MailModel>> GetSentMails(float updateInterval = default, bool forceUpdate = false);
    UniTask<List<MailModel>> GetWaitMails(float updateInterval = default, bool forceUpdate = false);
    MailModel GetMail(string mailId);
}

