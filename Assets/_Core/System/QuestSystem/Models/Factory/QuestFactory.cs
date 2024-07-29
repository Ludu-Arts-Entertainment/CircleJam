#if !QuestManager_Modified
using System.Collections.Generic;

public static class QuestFactory
{
    private static readonly Dictionary<QuestType, BaseQuest> Quest = new Dictionary<QuestType, BaseQuest>()
    {
        { QuestType.EarnXCurrencyFromStart , new EarnXCurrencyFromStartQuest()},
        { QuestType.SpendXCurrencyFromStart, new SpendXCurrencyFromStartQuest() },
        { QuestType.EarnXCurrencyFromActivation, new EarnXCurrencyFromActivationQuest() },
        { QuestType.SpendXCurrencyFromActivation, new SpendXCurrencyFromActivationQuest() },
        { QuestType.HaveXCurrency, new HaveXCurrencyQuest() },
        
    };

    public static BaseQuest Create(IQuestGroupController questGroupController ,Quest quest)
    {
        return Quest.TryGetValue(quest.Type, out var tQuest)==false ? null : tQuest.Create(questGroupController, quest.Clone());
    }
       
}
#endif