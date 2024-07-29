#if PlayFabSdk_Enabled
using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
 
public class PlayfabMailProvider : IMailProvider
{
    private const float waitListUpdateInterval = 30f;
    private const float sentListUpdateInterval = 30f;

    private const string sentMailDataKey = "SentMails";
    private const string waitMailDataKey = "WaitingMails";
    
    private enum FunctionNames
    {
        Send,
        Delete,
        Read,
        Claim,
        DeleteManyFromInbox,
        DeleteManyFromSentbox
    }
    private Dictionary<FunctionNames, string> FunctionNameMapper => new ()
    {
        {FunctionNames.Send, "SendMailRequest"},
        {FunctionNames.Delete, "DeleteMailRequest"},
        {FunctionNames.Read, "ChangeMailStatusRequest"},
        {FunctionNames.Claim, "ChangeMailStatusRequest"},
        {FunctionNames.DeleteManyFromInbox, "DeleteMoreInboxRequest"},
        {FunctionNames.DeleteManyFromSentbox, "DeleteMoreSentboxRequest"}
    };
    
    private float _lastWaitUpdateTime;
    private float _lastSentUpdateTime;
    
    private readonly Dictionary<string, Dictionary<string, MailModel>> _waitingMails = new();
    private readonly Dictionary<string, Dictionary<string, MailModel>> _sentMails = new();
    public IMailProvider CreateSelf()
    {
        return new PlayfabMailProvider();
    }

    public IMailProvider.MailListUpdate OnSentMailListUpdated { get; set; }
    public IMailProvider.MailListUpdate OnWaitMailListUpdated { get; set; }
    public IMailProvider.MailUpdate OnMailDeleted { get; set; }
    public IMailProvider.MailUpdate OnMailRead { get; set; }
    public IMailProvider.MailUpdate OnMailClaimed { get; set; }

    public void SendMail(MailModel mailModel, Action<RequestResponse> onSuccess, Action<RequestResponse> onError)
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = FunctionNameMapper[FunctionNames.Send], // Arbitrary function name (must exist in your uploaded cloud.js file)
                FunctionParameter = mailModel,
                GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
            },
            OnRequestSuccess, OnRequestFail);

        void OnRequestSuccess(ExecuteCloudScriptResult e)
        {
            var response = PlayFabHelper.ResponseHandling(e);
            if (response.Success)
            {
                var id = ((JObject)response.Data).TryGetValue("id", out var idValue) ? idValue : 0;
                mailModel.id = id.First.ToString();
                _sentMails.TryAdd(mailModel.ReceiverId, new Dictionary<string, MailModel>() { });
                _sentMails[mailModel.ReceiverId].TryAdd(mailModel.id, mailModel);
                onSuccess?.Invoke(response);
                OnSentMailListUpdated?.Invoke();
            }
            else
            {
                onError?.Invoke(response);
            }
        }
        void OnRequestFail(PlayFabError error)
        {
            onError?.Invoke(PlayFabHelper.ResponseHandling(error));
        }
    }
    public void DeleteMail(string senderId, string mailId, Action<RequestResponse> onSuccess, Action<RequestResponse> onError)
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = FunctionNameMapper[FunctionNames.Delete], // Arbitrary function name (must exist in your uploaded cloud.js file)
                FunctionParameter = new
                {
                    senderId = senderId,
                    id = mailId
                },
                GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
            },
            OnRequestSuccess, OnRequestFail);

        void OnRequestSuccess(ExecuteCloudScriptResult e)
        {
            var response = PlayFabHelper.ResponseHandling(e);
            if (response.Success)
            {
                _waitingMails.TryAdd(senderId, new Dictionary<string, MailModel>() { });
                _waitingMails[senderId].Remove(mailId);
                onSuccess?.Invoke(response);
                OnMailDeleted?.Invoke(mailId);
                OnWaitMailListUpdated?.Invoke();
            }
            else
            {
                onError?.Invoke(response);
            }
        }
        void OnRequestFail(PlayFabError error)
        {
            onError?.Invoke(PlayFabHelper.ResponseHandling(error));
        }
    }
    public void ReadMail(string senderId, string mailId, Action<RequestResponse> onSuccess, Action<RequestResponse> onError)
    {
        ChangeMailStatus(senderId, mailId, MailStatus.Read, onSuccess, onError);
    }
    public void ClaimMail(string senderId, string mailId, Action<RequestResponse> onSuccess, Action<RequestResponse> onError)
    {
        ChangeMailStatus(senderId, mailId, MailStatus.Claimed, onSuccess, onError);
    }
    private void ChangeMailStatus(string senderId, string mailId, MailStatus status, Action<RequestResponse> onSuccess, Action<RequestResponse> onError)
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = FunctionNameMapper[status == MailStatus.Read ? FunctionNames.Read : FunctionNames.Claim], // Arbitrary function name (must exist in your uploaded cloud.js file)
                FunctionParameter = new
                {
                    senderId = senderId,
                    id = mailId,
                    status = status
                },
                GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
            },
            OnRequestSuccess, OnRequestFail);

        void OnRequestSuccess(ExecuteCloudScriptResult e)
        {
            var response = PlayFabHelper.ResponseHandling(e);
            if (response.Success)
            {
                _waitingMails.TryAdd(senderId, new Dictionary<string, MailModel>() { });
                _waitingMails[senderId][mailId].Status = status;
                if (status == MailStatus.Claimed)
                {
                    OnMailClaimed?.Invoke(mailId);
                    if (_waitingMails[senderId][mailId].IsClaimAndDelete)
                    {
                        _waitingMails[senderId].Remove(mailId);
                        OnMailDeleted?.Invoke(mailId);
                    }
                }
                else
                {
                    OnMailRead?.Invoke(mailId);
                }
                onSuccess?.Invoke(response);
                OnWaitMailListUpdated?.Invoke();
            }
            else
            {
                onError?.Invoke(response);
            }
        }
        void OnRequestFail(PlayFabError error)
        {
            onError?.Invoke(PlayFabHelper.ResponseHandling(error));
        }
    }
    /// <summary>
    /// 5
    /// </summary>
    /// <param name="mails"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    private async UniTask DeleteMoreFromInbox(string senderId, string[] mailIds , Action<RequestResponse> onSuccess, Action<RequestResponse> onError)
    {
        var isFinished = false;
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = FunctionNameMapper[FunctionNames.DeleteManyFromInbox], // Arbitrary function name (must exist in your uploaded cloud.js file)
                FunctionParameter = new
                {
                    senderId = senderId,
                    ids = mailIds
                },
                GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
            },
            OnRequestSuccess, OnRequestFail);

        void OnRequestSuccess(ExecuteCloudScriptResult e)
        {
            var response = PlayFabHelper.ResponseHandling(e);
            if (response.Success)
            {
                if (!_waitingMails.ContainsKey(senderId))return;
                foreach (var mailId in mailIds)
                {
                    if (_waitingMails[senderId].ContainsKey(mailId))
                    {
                        OnMailDeleted?.Invoke(mailId);
                        _waitingMails[senderId].Remove(mailId);
                    }
                }
                onSuccess?.Invoke(response);
                OnWaitMailListUpdated?.Invoke();
            }
            else
            {
                onError?.Invoke(response);
            }
            isFinished = true;
        }
        void OnRequestFail(PlayFabError error)
        {
            isFinished = true;
            onError?.Invoke(PlayFabHelper.ResponseHandling(error));
        }
        await UniTask.WaitUntil(() => isFinished);
    }
    private async UniTask DeleteMoreFromSentBox(string receiverId, string[] mailIds , Action<RequestResponse> onSuccess, Action<RequestResponse> onError)
    {
        var isFinished = false;
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = FunctionNameMapper[FunctionNames.DeleteManyFromSentbox], // Arbitrary function name (must exist in your uploaded cloud.js file)
                FunctionParameter = new
                {
                    receiverId = receiverId,
                    ids = mailIds
                },
                GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
            },
            OnRequestSuccess, OnRequestFail);

        void OnRequestSuccess(ExecuteCloudScriptResult e)
        {
            var response = PlayFabHelper.ResponseHandling(e);
            if (response.Success)
            {
                if (!_sentMails.ContainsKey(receiverId))return;
                foreach (var mailId in mailIds)
                {
                    if (_sentMails[receiverId].ContainsKey(mailId))
                    {
                        OnMailDeleted? .Invoke(mailId); _sentMails[receiverId].Remove(mailId);
                    }
                }
                onSuccess?.Invoke(response);
                OnSentMailListUpdated?.Invoke();
            }
            else
            {
                onError?.Invoke(response);
            }
            isFinished = true;
        }
        void OnRequestFail(PlayFabError error)
        {
            isFinished = true;
            onError?.Invoke(PlayFabHelper.ResponseHandling(error));
        }
        await UniTask.WaitUntil(() => isFinished);
    }
    public async UniTask<List<MailModel>> GetSentMails(float updateInterval = default, bool forceUpdate = false)
    {
        bool finished = false;
        if (_sentMails is { Count: 0} ||forceUpdate || _lastSentUpdateTime + (updateInterval != default?updateInterval:sentListUpdateInterval) <= Time.time)
        {
            PlayFabClientAPI.GetUserData(new GetUserDataRequest()
            {
                Keys = new List<string>() { sentMailDataKey }
            }, OnRequestSuccess, OnRequestError);
        }else finished = true;
        void OnRequestSuccess(GetUserDataResult obj)
        {
            if (obj.Data.TryGetValue(sentMailDataKey, out var data))
            {
                var jsonResult = JsonHelper.FromJson(data.Value);
                JObject jsonObject = (JObject)jsonResult;
                var properties = jsonObject.Properties();
                _sentMails.Clear();
                foreach (var jProperty in properties)
                {
                    _sentMails.TryAdd(jProperty.Name, new Dictionary<string, MailModel>() { });
                    (jProperty.Value as JObject).Properties().ToList().ForEach(x =>
                    {
                        var mailModel = x.Value.ToObject<MailModel>();
                        _sentMails[jProperty.Name].TryAdd(x.Name, mailModel);
                    });
                }
                _lastSentUpdateTime = Time.time;
                OnSentMailListUpdated?.Invoke();
            }
            finished = true;
        }

        void OnRequestError(PlayFabError error)
        {
            finished = true;
        }
        
        await UniTask.WaitUntil(() => finished);
        await CheckSentBoxExpires();
        
        return _sentMails.SelectMany(x => x.Value.Values).ToList();
    }
    public async UniTask<List<MailModel>> GetWaitMails(float updateInterval = default, bool forceUpdate = false)
    {
        bool finished = false;
        if (_waitingMails is { Count: 0} ||forceUpdate || _lastWaitUpdateTime + (updateInterval != default?updateInterval:waitListUpdateInterval) <= Time.time)
        {
            PlayFabClientAPI.GetUserData(new GetUserDataRequest()
            {
                Keys = new List<string>() { waitMailDataKey }
            }, OnRequestSuccess, OnRequestError);
        }else finished = true;
        void OnRequestSuccess(GetUserDataResult obj)
        {
            if (obj.Data.TryGetValue(waitMailDataKey, out var data))
            {
                var jsonResult = JsonHelper.FromJson(data.Value);
                JObject jsonObject = (JObject)jsonResult;
                var properties = jsonObject.Properties();
                _waitingMails.Clear();
                foreach (var jProperty in properties)
                {
                    _waitingMails.TryAdd(jProperty.Name, new Dictionary<string, MailModel>() { });
                    (jProperty.Value as JObject).Properties().ToList().ForEach(x =>
                    {
                        var mailModel = x.Value.ToObject<MailModel>();
                        _waitingMails[jProperty.Name].TryAdd(x.Name, mailModel);
                    });
                }
                _lastWaitUpdateTime = Time.time;
                OnWaitMailListUpdated?.Invoke();
            }
            finished = true;
        }
        void OnRequestError(PlayFabError error)
        {
            finished = true;
        }
        await UniTask.WaitUntil(() => finished);
        await CheckInboxExpires();
        return _waitingMails.SelectMany(x => x.Value.Values).ToList();
    }
    private async UniTask CheckInboxExpires()
    {
        long currentTimestamp = (long)TimeHelper.DateTimeToUnixTimeStampInSeconds(TimeHelper.GetCurrentDateTime());
        var expiredMails = _waitingMails.SelectMany(x => x.Value.Values).Where(x => x.ExpireTimestamp != -1 && x.ExpireTimestamp <= currentTimestamp).ToList();
        if (expiredMails.Count <= 0) return;
        var mails = new Dictionary<string, string[]>();
        expiredMails.ForEach(x =>
        {
            if (mails.ContainsKey(x.SenderId))
            {
                mails[x.SenderId] = mails[x.SenderId].Append(x.id).ToArray();
            }
            else
            {
                mails.Add(x.SenderId, new string[] {x.id});
            }
        });
        if (mails.Count <= 0) return;
        foreach (var mail in mails)
        {
            await DeleteMoreFromInbox(mail.Key, mail.Value, null,null);
        }
    }
    private async UniTask CheckSentBoxExpires()
    {        
        long currentTimestamp = (long)TimeHelper.DateTimeToUnixTimeStampInSeconds(TimeHelper.GetCurrentDateTime());
        var expiredMails = _sentMails.SelectMany(x => x.Value.Values).Where(x => x.ExpireTimestamp !=-1 && x.ExpireTimestamp <= currentTimestamp).ToList();
        if (expiredMails.Count <= 0) return;
        var mails = new Dictionary<string, string[]>();
        expiredMails.ForEach(x =>
        {
            if (mails.ContainsKey(x.ReceiverId))
            {
                mails[x.ReceiverId] = mails[x.ReceiverId].Append(x.id).ToArray();
            }
            else
            {
                mails.Add(x.ReceiverId, new string[] {x.id});
            }
        });
        if (mails.Count <= 0) return;
        foreach (var mail in mails)
        {
            await DeleteMoreFromSentBox(mail.Key, mail.Value, null,null);
        }
    }
    public MailModel GetMail(string mailId)
    {
        return _waitingMails.SelectMany(x => x.Value.Values).FirstOrDefault(x => x.id == mailId) ??
               _sentMails.SelectMany(x => x.Value.Values).FirstOrDefault(x => x.id == mailId);
    }
}
#endif