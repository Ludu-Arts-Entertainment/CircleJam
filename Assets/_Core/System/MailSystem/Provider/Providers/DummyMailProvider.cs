using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class DummyMailProvider : IMailProvider
{
    public IMailProvider CreateSelf()
    {
        return new DummyMailProvider();
    }

    public IMailProvider.MailListUpdate OnSentMailListUpdated { get; set; }
    public IMailProvider.MailListUpdate OnWaitMailListUpdated { get; set; }
    public IMailProvider.MailUpdate OnMailDeleted { get; set; }
    public IMailProvider.MailUpdate OnMailRead { get; set; }
    public IMailProvider.MailUpdate OnMailClaimed { get; set; }
    public void SendMail(MailModel mailModel, Action<RequestResponse> onSuccess, Action<RequestResponse> onError)
    {
        throw new NotImplementedException();
    }

    public void DeleteMail(string senderId, string mailId, Action<RequestResponse> onSuccess, Action<RequestResponse> onError)
    {
        throw new NotImplementedException();
    }

    public void ReadMail(string senderId, string mailId, Action<RequestResponse> onSuccess, Action<RequestResponse> onError)
    {
        throw new NotImplementedException();
    }

    public void ClaimMail(string senderId, string mailId, Action<RequestResponse> onSuccess, Action<RequestResponse> onError)
    {
        throw new NotImplementedException();
    }

    public UniTask<List<MailModel>> GetSentMails(float updateInterval = default, bool forceUpdate = false)
    {
        throw new NotImplementedException();
    }

    public UniTask<List<MailModel>> GetWaitMails(float updateInterval = default, bool forceUpdate = false)
    {
        throw new NotImplementedException();
    }

    public MailModel GetMail(string mailId)
    {
        throw new NotImplementedException();
    }
}
