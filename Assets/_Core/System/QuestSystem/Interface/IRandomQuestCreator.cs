using System.Collections.Generic;

public interface IRandomQuestCreator
{
    Quest Create(string questGroupId, List<Quest> quests);
    List<QuestType> CreatableQuestTypes { get; }
}