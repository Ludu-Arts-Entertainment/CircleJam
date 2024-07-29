using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicQuestCreator : IRandomQuestCreator
{
    public Quest Create(string questGroupId, List<Quest> quests)
    {
        var creatableQuestTypes = new List<QuestType>(CreatableQuestTypes);
        var randomQuestType = CreatableQuestTypes[UnityEngine.Random.Range(0, CreatableQuestTypes.Count)];
        var questIndex = quests.FindLastIndex(x => x.Type == randomQuestType);
        while (questIndex ==-1)
        {
            creatableQuestTypes.Remove(randomQuestType);
            randomQuestType = CreatableQuestTypes[UnityEngine.Random.Range(0, CreatableQuestTypes.Count)];
            questIndex = quests.FindLastIndex(x => x.Type == randomQuestType);
            if (creatableQuestTypes.Count==0)
            {
                return null;
            }
        }
        var quest = quests[questIndex].Clone();
        var parsedId = quest.Id.Split('_');
        var queue = 0;
        if (parsedId[1]==questGroupId && parsedId[2]==randomQuestType.ToString())
        {
            queue = Convert.ToInt32(parsedId[^1])+1;
        }
        quest.Id = $"{DateTime.Now}_{questGroupId}_{quest.Type}_{queue}";
        quest.metaData.Title = $"Random {quest.metaData.Title}";
        quest.QuestData.Amount *= 10;
        quest.CurrentAmount = 0;
        quest.State = QuestState.Waiting;
        quest.Order = quests[^1].Order + 1;
        return quest;
    }

    public List<QuestType> CreatableQuestTypes { get; } = new List<QuestType>() {
        QuestType.EarnXCurrencyFromStart,
        QuestType.SpendXCurrencyFromStart,
        QuestType.EarnXCurrencyFromActivation,
        QuestType.SpendXCurrencyFromActivation,
        QuestType.HaveXCurrency
    };
}
