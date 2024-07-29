using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameData
{
    public Dictionary<QuestGroupStatus,Dictionary<string,QuestGroupInfo>> QuestData = new ()
    {
        {QuestGroupStatus.None, new Dictionary<string, QuestGroupInfo>()},
        {QuestGroupStatus.Active, new Dictionary<string, QuestGroupInfo>()},
        {QuestGroupStatus.DeActive, new Dictionary<string, QuestGroupInfo>()},
        {QuestGroupStatus.Completed, new Dictionary<string, QuestGroupInfo>()}
    };
}
