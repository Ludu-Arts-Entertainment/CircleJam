using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestContainer", menuName = "Quest/QuestContainer")]
public class QuestContainer : ScriptableObject
{
    public List<Quest> Quests = new ();
    public List<Quest> Clone()
    {
        var cloneQuests = new List<Quest>();
        foreach (var quest in Quests)
        {
            cloneQuests.Add(quest.Clone());
        }

        return cloneQuests;
    }
}