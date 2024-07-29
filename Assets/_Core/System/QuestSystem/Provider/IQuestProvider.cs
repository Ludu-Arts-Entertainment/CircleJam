using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IQuestProvider
{
    IQuestProvider CreateSelf();
    void Initialize(Action onReady);

    #region Quest State Management

    void QuestProgressChange(Quest quest, string questGroupId);
    void QuestActivate(Quest quest, string questGroupId);
    void QuestComplete(Quest quest, string questGroupId);
    void QuestClaim(Quest quest, string questGroupId);

    #endregion

    #region Get Quests

    List<Quest> GetQuests(QuestState questState, string questGroupId,
        QuestGroupStatus questGroupStatus = QuestGroupStatus.Active);

    Dictionary<string, QuestGroupInfo> GetQuests(QuestState questState,
        QuestGroupStatus questGroupStatus = QuestGroupStatus.Active);

    Dictionary<QuestState, List<Quest>> GetQuests(string questGroupId,
        QuestGroupStatus questGroupStatus = QuestGroupStatus.Active);

    #endregion

    #region Quest Group Management

    void DeleteQuestGroup(string questGroupId);
    void ActivateQuestGroup(string questGroupId, QuestGroupInfo questGroupInfo);
    void DeactivateQuestGroup(string questGroupId);

    #endregion

    #region Quest Events
    public delegate void QuestStateChange(Quest quest, string questGroupId);
    public QuestStateChange OnQuestProgressChange { get; set; }
    public QuestStateChange OnQuestActivate { get; set; }
    public QuestStateChange OnQuestComplete { get; set; }
    public QuestStateChange OnQuestClaim { get; set; }
    #endregion

    #region Quest Group Events
    public delegate void QuestGroupStateChange(string questGroupId);
    public QuestGroupStateChange OnQuestGroupActivate { get; set; }
    public QuestGroupStateChange OnQuestGroupDeactivate { get; set; }
    public QuestGroupStateChange OnQuestGroupDelete { get; set; }
    public QuestGroupStateChange OnQuestGroupComplete { get; set; }
    
    #endregion

    IQuestGroupController GetActiveQuestGroupController(string questGroupId);
    IQuestGroupController GetActiveQuestGroupController(QuestGroupEnums questGroupEnums);
    QuestGroupInfo GetQuestGroupInfo(string questGroupId, QuestGroupStatus questGroupStatus = QuestGroupStatus.Active);
}