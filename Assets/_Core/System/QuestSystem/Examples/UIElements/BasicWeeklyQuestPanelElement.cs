using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BasicWeeklyQuestPanelElement : BaseUIElement
{
    #region UIElements

    [SerializeField] private Image questIcon;
    [SerializeField] private TMP_Text questProgressText;
    [SerializeField] private TMP_Text questGroupCooldownText;
    [SerializeField] private Image questProgressSlider;
    [SerializeField] private Image questGroupCompleteImage;

    [SerializeField] private Button claimButton;

    #endregion

    #region Managers

    private QuestManager _questManager;

    #endregion

    private string WeeklyQuestPanelElementTimerRoutineKey => "WeeklyQuestPanelElement_RoutineKey_" + GetInstanceID();
    private string _activeQuestGroupId;
    private IQuestGroupController _activeQuestGroupController;
    private bool _isInitialized;

    private void Start()
    {
        Initialize();
    }

    private void OnEnable()
    {
        claimButton.onClick.AddListener(Claim);
        Initialize();
    }

    private void OnDisable()
    {
        claimButton.onClick.RemoveListener(Claim);
        Dispose();
    }

    public override void Dispose()
    {
        if (!_isInitialized) return;
        _isInitialized = false;

        if (CoroutineController.IsCoroutineRunning(WeeklyQuestPanelElementTimerRoutineKey))
        {
            CoroutineController.StopCoroutine(WeeklyQuestPanelElementTimerRoutineKey);
        }
    }

    public override void Initialize()
    {
        if (_isInitialized) return;
        claimButton.gameObject.SetActive(false);
        _questManager ??= GameInstaller.Instance.SystemLocator.QuestManager;

        if (CoroutineController.IsCoroutineRunning(WeeklyQuestPanelElementTimerRoutineKey))
        {
            CoroutineController.StopCoroutine(WeeklyQuestPanelElementTimerRoutineKey);
        }

        _activeQuestGroupController =
            _questManager.GetActiveQuestGroupController(QuestGroupEnums.Weekly);

        var endTime = _questManager.GetQuestGroupInfo(_activeQuestGroupController.QuestGroup.Id).EndTime;
        CoroutineController.DoAfterCondition(() =>
        {
            var time = endTime - TimeHelper.GetCurrentDateTime();
            questGroupCooldownText.text = time.Days > 0
                ? $"{time.Days}d {time.Hours}h {time.Minutes}m"
                : $"{time.Hours}h {time.Minutes}m {time.Seconds}s";
            return endTime < TimeHelper.GetCurrentDateTime();
        }, null, WeeklyQuestPanelElementTimerRoutineKey);

        var getQuest = _activeQuestGroupController.GetQuests(new List<QuestState>()
            { QuestState.Active, QuestState.Completed, QuestState.Claimed });

        if (getQuest.Count > 0)
        {
            FillProperties(getQuest?.First());
        }

        questGroupCompleteImage.enabled = _activeQuestGroupController.GetQuests(new List<QuestState>()
            { QuestState.Completed, QuestState.Waiting, QuestState.Active }).Count == 0;

        _questManager.OnQuestProgressChange += OnQuestProgressChanged;
        _questManager.OnQuestGroupComplete += OnQuestGroupComplete;

        _isInitialized = true;
    }

    private void OnQuestGroupComplete(string questGroupId)
    {
        if (questGroupId == _activeQuestGroupController.QuestGroup.Id)
        {
            questGroupCompleteImage.enabled = true;
            claimButton.gameObject.SetActive(false);
        }
    }

    private void OnQuestProgressChanged(Quest quest, string questGroupId)
    {
        if (questGroupId == _activeQuestGroupController.QuestGroup.Id)
        {
            FillProperties(quest);
        }
    }

    private void FillProperties(Quest quest)
    {
        questProgressText.text = $"{quest.metaData.Title} : {quest.CurrentAmount}/{quest.QuestData.Amount}";
        questProgressSlider.DOFillAmount(quest.CurrentAmount / (float)quest.QuestData.Amount,
            1.3f);
        var wActiveQuestGroupController =
            GameInstaller.Instance.SystemLocator.QuestManager.GetActiveQuestGroupController(QuestGroupEnums
                .Weekly);
        var completedQuests = wActiveQuestGroupController.GetQuests(QuestState.Completed);
        claimButton.gameObject.SetActive(completedQuests.Count > 0);
    }

    public void Claim()
    {
        var completedQuests = _questManager.GetQuests(QuestState.Completed, _activeQuestGroupController.QuestGroup.Id);
        if (completedQuests.Count == 0) return;
        _activeQuestGroupController.QuestClaim(completedQuests.First());

        var profileStatData =
            GameInstaller.Instance.SystemLocator.DataManager.GetData<Dictionary<string, int>>(GameDataType
                .PlayerStatData);

        GameInstaller.Instance.SystemLocator.DataManager.SetData(GameDataType.PlayerStatData,
            profileStatData);
    
        claimButton.gameObject.SetActive(false);
    }
}