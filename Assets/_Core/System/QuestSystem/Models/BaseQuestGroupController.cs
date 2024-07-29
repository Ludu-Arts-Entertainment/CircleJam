using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BaseQuestGroupController : IQuestGroupController
{
    public List<BaseQuest> CreatedQuests { get; private set; }
    public QuestGroup QuestGroup { get; private set; }

    #region Managers

    protected QuestManager QuestManager;

    #endregion

    public abstract IQuestGroupController Create();


    public virtual void Initialize(QuestGroup questGroup, Dictionary<QuestState, List<Quest>> quests)
    {
        QuestManager = GameInstaller.Instance.SystemLocator.QuestManager;
        CreatedQuests = new List<BaseQuest>();
        QuestGroup = questGroup;
        var tActiveQuests = new List<Quest>();
        if (questGroup.IsCountedCompleted)
        {
            tActiveQuests.AddRange(quests[QuestState.Completed]);
        }

        tActiveQuests.AddRange(quests[QuestState.Active]);

        var willCreateNewQuestCount = QuestGroup.ActiveQuestCount - tActiveQuests.Count;
        willCreateNewQuestCount = willCreateNewQuestCount < 0 ? 0 : willCreateNewQuestCount;

        var getQuests = quests[QuestState.Waiting];
        if (willCreateNewQuestCount > getQuests.Count) willCreateNewQuestCount = getQuests.Count;

        tActiveQuests.AddRange(getQuests.GetRange(0, willCreateNewQuestCount));

        foreach (var quest in tActiveQuests)
        {
            if (quest.State == QuestState.Waiting) QuestManager.QuestActivate(quest, QuestGroup.Id);
            CreateQuest(quest);
        }
    }


    private void CreateQuest(Quest quest)
    {
        var baseQuest = QuestFactory.Create(this, quest);
        if (baseQuest == null) return;
        CreatedQuests.Add(baseQuest);
    }

    public List<Quest> GetQuests(QuestState questState = QuestState.None)
    {
        return QuestManager.GetQuests(questState, QuestGroup.Id);
    }

    public List<Quest> GetQuests(List<QuestState> questStates = null)
    {
        var quests = new List<Quest>();
        if (questStates == null)
        {
            foreach (var questState in Enum.GetNames(typeof(QuestState)))
            {
                quests.AddRange(GetQuests((QuestState)Enum.Parse(typeof(QuestState), questState)));
            }
        }
        else
        {
            foreach (var questState in questStates)
            {
                quests.AddRange(GetQuests(questState));
            }
        }

        return quests;
    }

    public void QuestDispose()
    {
        foreach (var quest in CreatedQuests)
        {
            quest.Dispose();
        }

        CreatedQuests.Clear();
    }

    public void QuestDispose(Quest quest)
    {
        var baseQuest = CreatedQuests.FirstOrDefault(x => x.Quest.Id == quest.Id);
        baseQuest?.Dispose();
        CreatedQuests.Remove(baseQuest);
    }

    public virtual void QuestClaim(Quest quest)
    {
        GameInstaller.Instance.SystemLocator.QuestManager.QuestClaim(quest, QuestGroup.Id);
        var baseQuest = CreatedQuests.FirstOrDefault(x => x.Quest == quest);
        CreatedQuests.Remove(baseQuest);
    }

    protected void ActivateNextQuest()
    {
        var questList = GetQuests(QuestState.Waiting);
        if (questList.Count == 0)
        {
            if (QuestGroup.IsFillWithRandom)
            {
                var claimedQuests = GetQuests(QuestState.Claimed);
                foreach (var baseQuest in CreatedQuests)
                {
                    claimedQuests.Add(baseQuest.Quest);
                }

                var createdQuest = RandomQuestFactory.Create(this, claimedQuests);
                QuestManager.QuestActivate(createdQuest, QuestGroup.Id);
                CreateQuest(createdQuest);
                return;
            }

            QuestManager.OnQuestGroupComplete(QuestGroup.Id);
            return;
        }

        var quest = questList[0];
        QuestManager.QuestActivate(quest, QuestGroup.Id);
        CreateQuest(quest);
    }
}