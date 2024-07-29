#if !QuestManager_Modified
using System.Collections.Generic;
public static class RandomQuestFactory
{
    private static Dictionary<RandomQuestGeneratorType, IRandomQuestCreator> _creators = new()
    {
        {RandomQuestGeneratorType.None, null},
    };
    
    public static Quest Create(IQuestGroupController questGroupController, List<Quest> quests)
    {
        if (_creators.TryGetValue(questGroupController.QuestGroup.RandomQuestCreator, out var generator))
        {
            return generator.Create(questGroupController.QuestGroup.Id, quests);
        }
        return null;
    }
}
public enum RandomQuestGeneratorType
{
    None,
    Basic
}
#endif