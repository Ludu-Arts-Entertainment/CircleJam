using System;
using System.Collections.Generic;

public interface IQuestGroupController
{
    IQuestGroupController Create();
    QuestGroup QuestGroup { get; }
    List<BaseQuest> CreatedQuests { get;}
    List<Quest> GetQuests(QuestState questState = QuestState.None);
    List<Quest> GetQuests(List<QuestState> questStates);
    void Initialize(QuestGroup questGroup, Dictionary<QuestState,List<Quest>> quests);
    void QuestClaim(Quest quest);
    void QuestDispose();
}