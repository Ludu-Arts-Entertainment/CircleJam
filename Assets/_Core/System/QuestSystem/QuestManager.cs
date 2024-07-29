using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : IManager
{
    private IQuestProvider _questProvider;

    #region Quest Events

    public IQuestProvider.QuestStateChange OnQuestProgressChange
    {
        get => _questProvider.OnQuestProgressChange;
        set => _questProvider.OnQuestProgressChange = value;
    }

    public IQuestProvider.QuestStateChange OnQuestActivate
    {
        get => _questProvider.OnQuestActivate;
        set => _questProvider.OnQuestActivate = value;
    }

    public IQuestProvider.QuestStateChange OnQuestComplete
    {
        get => _questProvider.OnQuestComplete;
        set => _questProvider.OnQuestComplete = value;
    }

    public IQuestProvider.QuestStateChange OnQuestClaim
    {
        get => _questProvider.OnQuestClaim;
        set => _questProvider.OnQuestClaim = value;
    }

    #endregion

    #region Quest Group Events

    public IQuestProvider.QuestGroupStateChange OnQuestGroupActivate
    {
        get => _questProvider.OnQuestGroupActivate;
        set => _questProvider.OnQuestGroupActivate = value;
    }

    public IQuestProvider.QuestGroupStateChange OnQuestGroupDeactivate
    {
        get => _questProvider.OnQuestGroupDeactivate;
        set => _questProvider.OnQuestGroupDeactivate = value;
    }

    public IQuestProvider.QuestGroupStateChange OnQuestGroupDelete
    {
        get => _questProvider.OnQuestGroupDelete;
        set => _questProvider.OnQuestGroupDelete = value;
    }
    public IQuestProvider.QuestGroupStateChange OnQuestGroupComplete
    {
        get => _questProvider.OnQuestGroupComplete;
        set => _questProvider.OnQuestGroupComplete = value;
    }

    #endregion

    public IManager CreateSelf()
    {
        return new QuestManager();
    }

    public void Initialize(GameInstaller gameInstaller, Action onReady)
    {
        _questProvider = QuestProviderFactory.Create(gameInstaller.Customizer.QuestProvider);
        _questProvider.Initialize(onReady);
    }

    public bool IsReady()
    {
        return _questProvider != null;
    }

    public void QuestProgressChange(Quest quest, string questGroupId)
    {
        _questProvider.QuestProgressChange(quest, questGroupId);
    }

    public void QuestActivate(Quest quest, string questGroupId)
    {
        _questProvider.QuestActivate(quest, questGroupId);
    }

    public void QuestComplete(Quest quest, string questGroupId)
    {
        _questProvider.QuestComplete(quest, questGroupId);
    }

    public void QuestClaim(Quest quest, string questGroupId)
    {
        _questProvider.QuestClaim(quest, questGroupId);
    }

    public List<Quest> GetQuests(QuestState questState, string questGroupId)
    {
        return _questProvider.GetQuests(questState, questGroupId);
    }

    public Dictionary<string, QuestGroupInfo> GetQuests(QuestState questState)
    {
        return _questProvider.GetQuests(questState);
    }

    public Dictionary<QuestState, List<Quest>> GetQuests(string questGroupId)
    {
        return _questProvider.GetQuests(questGroupId);
    }

    public void DeleteQuestGroup(string questGroupId)
    {
        _questProvider.DeleteQuestGroup(questGroupId);
    }

    public void ActivateQuestGroup(string questGroupId, QuestGroupInfo questGroupInfo)
    {
        _questProvider.ActivateQuestGroup(questGroupId, questGroupInfo);
    }

    public void DeActiveQuestGroup(string questGroupId)
    {
        _questProvider.DeactivateQuestGroup(questGroupId);
    }

    public IQuestGroupController GetActiveQuestGroupController(string questGroupId)
    {
        return _questProvider.GetActiveQuestGroupController(questGroupId);
    }
    public IQuestGroupController GetActiveQuestGroupController(QuestGroupEnums questGroupEnums)
    {
        return _questProvider.GetActiveQuestGroupController(questGroupEnums);
    }
    public QuestGroupInfo GetQuestGroupInfo(string questGroupId)
    {
        return _questProvider.GetQuestGroupInfo(questGroupId);
    }
}

public enum QuestState
{
    None = 0,
    Waiting = 1,
    Active = 2,
    Completed = 3,
    Claimed = 4
}

public enum QuestGroupStatus
{
    None = 1 << 1,
    Active = 1 << 2,
    DeActive = 1 << 3,
    Completed = 1 << 4,
}