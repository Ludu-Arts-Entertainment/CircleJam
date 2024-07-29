using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestProviderEnums
{
    BasicQuestProvider,
}
public static class QuestProviderFactory
{
    private static Dictionary<QuestProviderEnums,IQuestProvider> _questProviders = new ()
    {
        {QuestProviderEnums.BasicQuestProvider,new BasicQuestProvider()}
    };
    public static IQuestProvider Create(QuestProviderEnums providerEnum)
    {
        return _questProviders.TryGetValue(providerEnum, out var provider) ? provider.CreateSelf() : null;
    }
}
