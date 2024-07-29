using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestGroupCollection", menuName = "Quest/QuestGroupCollection")]
public class QuestGroupCollection:ScriptableObject
{
    public List<QuestGroup> QuestGroups = new List<QuestGroup>();
}