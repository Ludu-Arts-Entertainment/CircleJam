using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BasicQuestProvider : IQuestProvider
{
    #region Quest Events

    public IQuestProvider.QuestStateChange OnQuestProgressChange { get; set; }
    public IQuestProvider.QuestStateChange OnQuestActivate { get; set; }
    public IQuestProvider.QuestStateChange OnQuestComplete { get; set; }
    public IQuestProvider.QuestStateChange OnQuestClaim { get; set; }

    #endregion

    #region Quest Group Events

    public IQuestProvider.QuestGroupStateChange OnQuestGroupActivate { get; set; }
    public IQuestProvider.QuestGroupStateChange OnQuestGroupDeactivate { get; set; }
    public IQuestProvider.QuestGroupStateChange OnQuestGroupDelete { get; set; }
    public IQuestProvider.QuestGroupStateChange OnQuestGroupComplete { get; set; }

    #endregion

    private Dictionary<QuestGroupStatus, Dictionary<string, QuestGroupInfo>> _questDict = new();
    private List<IQuestGroupController> _activeQuestGroupControllers;
    private List<QuestGroup> _questGroupList = new();
    private readonly Dictionary<QuestGroupEnums, List<QuestGroup>> _questGroupEnumsDict = new();

    public IQuestProvider CreateSelf()
    {
        return new BasicQuestProvider();
    }

    public void Initialize(Action onReady)
    {
        Load();
        var questGroupCollection = Resources.Load<QuestGroupCollection>("QuestGroupCollection");
        _questGroupList = questGroupCollection?.QuestGroups;
        _questGroupList??= new List<QuestGroup>();
        _activeQuestGroupControllers = new List<IQuestGroupController>();
        FillPeriodicQuestGroupDict();
        var deletedQuestGroups = new List<string>();
        var deactivatedQuestGroups = new List<string>();
        var createdQuestGroups = new List<string>();
        foreach (var questState in _questDict)
        {
            foreach (var questGroup in questState.Value)
            {
                if (questGroup.Value.DeleteTime < TimeHelper.GetCurrentDateTime())
                {
                    deletedQuestGroups.Add(questGroup.Key);
                }
                else if (questState.Key == QuestGroupStatus.Active && questGroup.Value.EndTime < TimeHelper.GetCurrentDateTime())
                {
                    deactivatedQuestGroups.Add(questGroup.Key);
                }
                else if(questState.Key == QuestGroupStatus.Active)
                {
                    createdQuestGroups.Add(questGroup.Key);
                }
            }
        }

        deletedQuestGroups.ForEach(DeleteQuestGroup);
        deactivatedQuestGroups.ForEach(DeactivateQuestGroup);
        createdQuestGroups.ForEach(CreateQuestGroup);
        CalculateCurrentPeriods();
        onReady?.Invoke();
    }

    #region Data Management

    private void Load()
    {
        _questDict = new Dictionary<QuestGroupStatus, Dictionary<string, QuestGroupInfo>>(
            GameInstaller.Instance.SystemLocator.DataManager
                .GetData<Dictionary<QuestGroupStatus, Dictionary<string, QuestGroupInfo>>>
                    (GameDataType.QuestData));
    }

    private void Save()
    {
        GameInstaller.Instance.SystemLocator.DataManager.SetData(GameDataType.QuestData, _questDict);
    }

    #endregion

    #region Quest State Management

    public void QuestProgressChange(Quest quest, string questGroupId)
    {
        if (!_questDict[QuestGroupStatus.Active].ContainsKey(questGroupId))
        {
            Debug.Log("QuestProgressChange: QuestGroupId not found");
            return;
        }

        quest.UpdateTime = TimeHelper.GetCurrentDateTime();
        _questDict[QuestGroupStatus.Active][questGroupId].List.Find(x => x.Id == quest.Id).CurrentAmount =
            quest.CurrentAmount;
        Save();
        OnQuestProgressChange?.Invoke(quest, questGroupId);
        if (quest.CurrentAmount >= quest.QuestData.Amount)
        {
            QuestComplete(quest, questGroupId);
        }
    }

    public void QuestActivate(Quest quest, string questGroupId)
    {
        // Quest group not found in active quest list
        if (!_questDict[QuestGroupStatus.Active].ContainsKey(questGroupId)) return;
        // Check quest group for active quest list because quest group is not found in active quest list
        if (_questDict[QuestGroupStatus.Active][questGroupId].List.FindIndex(x => x.Id == quest.Id && x.State == QuestState.Active) != -1) return;
        // Add quest group to active quest list
        _questDict[QuestGroupStatus.Active][questGroupId].List.Find(x => x.Id == quest.Id).State = QuestState.Active;
        // Update quest progress time
        _questDict[QuestGroupStatus.Active][questGroupId].List.Find(x => x.Id == quest.Id).UpdateTime = TimeHelper.GetCurrentDateTime();
        // Save data
        Save();
        OnQuestActivate?.Invoke(_questDict[QuestGroupStatus.Active][questGroupId].List.Find(x => x.Id == quest.Id), questGroupId);
    }

    public void QuestComplete(Quest quest, string questGroupId)
    {
        // Quest group not found in active quest list
        if (!_questDict[QuestGroupStatus.Active].ContainsKey(questGroupId)) return;
        // Check quest group for active quest list because quest group is not found in active quest list
        if (_questDict[QuestGroupStatus.Active][questGroupId].List.FindIndex(x => x.Id == quest.Id) == -1) return;
        // Add quest group to active quest list
        _questDict[QuestGroupStatus.Active][questGroupId].List.Find(x => x.Id == quest.Id).State = QuestState.Completed;
        // Update quest progress time
        _questDict[QuestGroupStatus.Active][questGroupId].List.Find(x => x.Id == quest.Id).UpdateTime = TimeHelper.GetCurrentDateTime();

        // Save data
        Save();
        OnQuestComplete?.Invoke(_questDict[QuestGroupStatus.Active][questGroupId].List.Find(x => x.Id == quest.Id), questGroupId);
    }
    public void QuestClaim(Quest quest, string questGroupId)
    {
        // Quest group not found in active quest list
        if (!_questDict[QuestGroupStatus.Active].ContainsKey(questGroupId)) return;
        // Check quest group for active quest list because quest group is not found in active quest list
        if (_questDict[QuestGroupStatus.Active][questGroupId].List.FindIndex(x => x.Id == quest.Id) == -1) return;
        
        
        // Add quest group to active quest list
        _questDict[QuestGroupStatus.Active][questGroupId].List.Find(x => x.Id == quest.Id).State = QuestState.Claimed;
        // Update quest progress time
        _questDict[QuestGroupStatus.Active][questGroupId].List.Find(x => x.Id == quest.Id).UpdateTime = TimeHelper.GetCurrentDateTime();

        // Save data
        Save();
        OnQuestClaim?.Invoke(_questDict[QuestGroupStatus.Active][questGroupId].List.Find(x => x.Id == quest.Id), questGroupId);
    }

    #endregion

    #region Get Quests

    public List<Quest> GetQuests(QuestState questState, string questGroupId,
        QuestGroupStatus questGroupStatus = QuestGroupStatus.Active)
    {
        // Quest group not found in active quest groups list
        return !_questDict[questGroupStatus].ContainsKey(questGroupId)
            ? new List<Quest>()
            :
            // Check quest group for active quest list because quest group is not found in active quest list
            new List<Quest>(_questDict[questGroupStatus][questGroupId].List
                .Where(x => x.IsActive && x.State == questState));
    }

    public Dictionary<string, QuestGroupInfo> GetQuests(QuestState questState,
        QuestGroupStatus questGroupStatus = QuestGroupStatus.Active)
    {
        return _questDict[questGroupStatus].Where(x =>x.Value.List.FindIndex(y => y.State == questState) != -1)
            .ToDictionary(x => x.Key, x => x.Value);
    }

    /// <summary>
    /// 
    /// </summary>
    public Dictionary<QuestState, List<Quest>> GetQuests(string questGroupId,
        QuestGroupStatus questGroupStatus = QuestGroupStatus.Active)
    {
        var questGroupInfo = _questDict[questGroupStatus][questGroupId];
        var questList = questGroupInfo.List;
        var waitingQuests = questList.FindAll(x => x.State == QuestState.Waiting && x.IsActive);
        var activeQuests = questList.FindAll(x => x.State == QuestState.Active && x.IsActive);
        var completedQuests = questList.FindAll(x => x.State == QuestState.Completed && x.IsActive);
        var claimedQuests = questList.FindAll(x => x.State == QuestState.Claimed && x.IsActive);
        return new Dictionary<QuestState, List<Quest>>()
        {
            { QuestState.Waiting, waitingQuests },
            { QuestState.Active, activeQuests },
            { QuestState.Completed, completedQuests },
            { QuestState.Claimed, claimedQuests },
        };
    }

    #endregion

    #region Quest Group Management

    public void ActivateQuestGroup(string questGroupId, QuestGroupInfo questGroupInfo)
    {
        // Quest group found in active quest groups list
        if (_questDict[QuestGroupStatus.Active].ContainsKey(questGroupId)) return;

        var questGroup = _questGroupList.Find(x => x.Id == questGroupId);

        // Quest group not found in quest group list
        if (questGroup == null) return;
        questGroupInfo.List = questGroup.QuestContainer.Clone();
        _questDict[QuestGroupStatus.Active].Add(questGroupId, questGroupInfo);
        Save();

        CreateQuestGroup(questGroupId);
    }

    public void DeactivateQuestGroup(string questGroupId)
    {
        // Quest group not found in active quest groups list
        if (!_questDict[QuestGroupStatus.Active].ContainsKey(questGroupId)) return;

        var questGroupController = _activeQuestGroupControllers.Find(x => x.QuestGroup.Id == questGroupId);

        // Quest group controller not found in active quest group controllers list
        if (questGroupController != null)
        {
            questGroupController.QuestDispose();
            _activeQuestGroupControllers.Remove(questGroupController);
        }

        if (!_questDict[QuestGroupStatus.DeActive]
                .TryAdd(questGroupId, _questDict[QuestGroupStatus.Active][questGroupId]))
        {
            _questDict[QuestGroupStatus.DeActive][questGroupId] = _questDict[QuestGroupStatus.Active][questGroupId];
        }

        _questDict[QuestGroupStatus.Active].Remove(questGroupId);
        Save();
        OnQuestGroupDeactivate?.Invoke(questGroupId);
        CalculateCurrentPeriods();
    }


    public void DeleteQuestGroup(string questGroupId)
    {
        if (_questDict[QuestGroupStatus.Active].ContainsKey(questGroupId))
        {
            DeactivateQuestGroup(questGroupId);
        }

        _questDict[QuestGroupStatus.DeActive].Remove(questGroupId);
        Save();
        OnQuestGroupDelete?.Invoke(questGroupId);
    }

    private void CreateQuestGroup(string questGroupId)
    {
        var questGroup = _questGroupList.Find(x => x.Id == questGroupId);

        var questGroupController = QuestGroupFactory.Create(questGroup.QuestGroupEnum);

        questGroupController.Initialize(questGroup, GetQuests(questGroupId));

        _activeQuestGroupControllers.Add(questGroupController);

        CoroutineController.DoAfterCondition(
            () => _questDict[QuestGroupStatus.Active][questGroupId].EndTime < TimeHelper.GetCurrentDateTime(),
            () => { DeactivateQuestGroup(questGroupId); });
        OnQuestGroupActivate?.Invoke(questGroupId);
    }

    #endregion
    #region Periodic Quest Group Management

    private void FillPeriodicQuestGroupDict()
    {
        foreach (var questGroup in _questGroupList)
        {
            _questGroupEnumsDict.TryAdd(questGroup.QuestGroupEnum, new List<QuestGroup>());
            _questGroupEnumsDict[questGroup.QuestGroupEnum].Add(questGroup);
        }

        foreach (var keyValuePair in _questGroupEnumsDict)
        {
            keyValuePair.Value.Sort((x, y) => x.Order.CompareTo(y.Order));
        }
    }

    private void CalculateCurrentPeriods()
    {
        var currentTimeStamp = TimeHelper.DateTimeToUnixTimeStampInSeconds(TimeHelper.GetCurrentDateTime());
        foreach (var groupList in _questGroupEnumsDict.Values)
        {
            if((groupList.Count > 0 && groupList[0].IsPeriodic == false) || 
                groupList.Count < 0) continue;

            var periodTime = groupList[0].PeriodTime;
            var shift = groupList[0].Shift;
            var calculatedPeriod = (((long)currentTimeStamp-shift) / periodTime);
            var groupOrder = (int)(calculatedPeriod % groupList.Count);
            var group = groupList[groupOrder];
            var startTime = TimeHelper.UnixTimeStampToDateTime((ulong)(calculatedPeriod * periodTime+shift));
            ActivateQuestGroup(group.Id,
                new QuestGroupInfo(startTime, startTime.AddSeconds(group.LifeTime), group.QuestContainer.Quests,
                    startTime.AddSeconds(group.DeleteTime)));
        }
    }

    #endregion

    public IQuestGroupController GetActiveQuestGroupController(string questGroupId)
    {
        return _activeQuestGroupControllers.Find(x => x.QuestGroup.Id == questGroupId);
    }
    public IQuestGroupController GetActiveQuestGroupController(QuestGroupEnums questGroupEnums)
    {
        return _activeQuestGroupControllers.Find(x => x.QuestGroup.QuestGroupEnum == questGroupEnums);
    }
    public QuestGroupInfo GetQuestGroupInfo(string questGroupId, QuestGroupStatus questGroupStatus = QuestGroupStatus.Active)
    {
        return _questDict[questGroupStatus][questGroupId];
    }
}