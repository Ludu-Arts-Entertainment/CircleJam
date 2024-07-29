#if !QuestManager_Modified
using System.Collections.Generic;
public static class QuestGroupFactory
{
    private static readonly Dictionary<QuestGroupEnums, IQuestGroupController> QuestGroupControllers = new Dictionary<QuestGroupEnums, IQuestGroupController>()
    {
        { QuestGroupEnums.BasicGameQuest , new BasicQuestGroupController()},
        { QuestGroupEnums.Daily , new BasicDailyQuestGroupController()},
        { QuestGroupEnums.Weekly , new BasicWeeklyQuestGroupController()},
    };
    public static IQuestGroupController Create(QuestGroupEnums type)//(QuestGroup questGroup, List<Quest> activeQuests = null)
    {
        return QuestGroupControllers.TryGetValue(type, out var tQuestGroupController)==false ? null : tQuestGroupController.Create();
    }
}

public enum QuestGroupEnums
{
    BasicGameQuest,
    Daily,
    Weekly,
}
#endif
