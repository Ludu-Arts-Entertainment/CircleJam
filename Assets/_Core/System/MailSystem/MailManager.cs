using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class MailManager : IManager
{
    private IMailProvider _mailProvider;

    public IMailProvider.MailListUpdate OnSentMailListUpdated
    {
        get => _mailProvider.OnSentMailListUpdated;
        set => _mailProvider.OnSentMailListUpdated = value;
    }

    public IMailProvider.MailListUpdate OnWaitMailListUpdated
    {
        get => _mailProvider.OnWaitMailListUpdated;
        set => _mailProvider.OnWaitMailListUpdated = value;
    }
    public IMailProvider.MailUpdate OnMailDeleted
    {
        get => _mailProvider.OnMailDeleted;
        set => _mailProvider.OnMailDeleted = value;
    }
    public IMailProvider.MailUpdate OnMailRead
    {
        get => _mailProvider.OnMailRead;
        set => _mailProvider.OnMailRead = value;
    }
    public IMailProvider.MailUpdate OnMailClaimed
    {
        get => _mailProvider.OnMailClaimed;
        set => _mailProvider.OnMailClaimed = value;
    }

    public IManager CreateSelf()
    {
        return new MailManager();
    }

    public void Initialize(GameInstaller gameInstaller, Action onReady)
    {
        _mailProvider = MailProviderFactory.Create(gameInstaller.Customizer.MailProvider);
        onReady?.Invoke();
    }

    public bool IsReady()
    {
        return _mailProvider != null;
    }

    public void SendMail(MailModel mailModel, Action<RequestResponse> onSuccess, Action<RequestResponse> onError)
    {
        _mailProvider.SendMail(mailModel, onSuccess, onError);
    }

    public void DeleteMail(string senderId, string mailId, Action<RequestResponse> onSuccess,
        Action<RequestResponse> onError)
    {
        _mailProvider.DeleteMail(senderId, mailId, onSuccess, onError);
    }

    public void ReadMail(string senderId, string mailId, Action<RequestResponse> onSuccess,
        Action<RequestResponse> onError)
    {
        _mailProvider.ReadMail(senderId, mailId, onSuccess, onError);
    }

    public void ClaimMail(string senderId, string mailId, Action<RequestResponse> onSuccess,
        Action<RequestResponse> onError)
    {
        _mailProvider.ClaimMail(senderId, mailId, onSuccess, onError);
    }

    public UniTask<List<MailModel>> GetSentMails(float updateInterval = default, bool forceUpdate = false)
    {
        return _mailProvider.GetSentMails(updateInterval, forceUpdate);
    }

    public UniTask<List<MailModel>> GetWaitMails(float updateInterval = default, bool forceUpdate = false)
    {
        return _mailProvider.GetWaitMails(updateInterval, forceUpdate);
    }

    public MailModel GetMail(string mailId)
    {
        return _mailProvider.GetMail(mailId);
    }
}