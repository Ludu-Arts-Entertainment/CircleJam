using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BasicDailyQuestPanelElement : BaseUIElement
{
    [SerializeField] private Image _questIcon;
    [SerializeField] private TMPro.TMP_Text _questProgressText;
    [SerializeField] private TMPro.TMP_Text _questGroupCooldownText;
    [SerializeField] private Image _questProgressSlider;
    [SerializeField] private Image _questGroupCompleteImage;

    [SerializeField] private Button claimButton;
    private string DailyQuestPanelElementTimerRoutineKey => "DailyQuestPanelElement_RoutineKey_" + GetInstanceID();
    private IQuestGroupController _activeQuestGroupController;

    private bool _isInitialized;

    #region Managers

    private QuestManager _questManager;

    #endregion

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

        if (CoroutineController.IsCoroutineRunning(DailyQuestPanelElementTimerRoutineKey))
        {
            CoroutineController.StopCoroutine(DailyQuestPanelElementTimerRoutineKey);
        }
    }

    public override void Initialize()
    {
        if (_isInitialized) return;
        claimButton.gameObject.SetActive(false);
        _questManager ??= GameInstaller.Instance.SystemLocator.QuestManager;
        if (CoroutineController.IsCoroutineRunning(DailyQuestPanelElementTimerRoutineKey))
        {
            CoroutineController.StopCoroutine(DailyQuestPanelElementTimerRoutineKey);
        }

        _activeQuestGroupController =
            _questManager.GetActiveQuestGroupController(QuestGroupEnums.Daily);

        var endTime = _questManager.GetQuestGroupInfo(_activeQuestGroupController.QuestGroup.Id).EndTime;
        CoroutineController.DoAfterCondition(() =>
        {
            var time = endTime - TimeHelper.GetCurrentDateTime();
            _questGroupCooldownText.text = $"{time.Hours}h {time.Minutes}m {time.Seconds}s";
            return endTime < TimeHelper.GetCurrentDateTime();
        }, null, DailyQuestPanelElementTimerRoutineKey);

        var getQuest = _activeQuestGroupController.GetQuests(new List<QuestState>()
            { QuestState.Active, QuestState.Completed, QuestState.Claimed });
        if (getQuest.Count > 0)
        {
            FillProperties(getQuest.First());
        }

        _questGroupCompleteImage.enabled = _activeQuestGroupController.GetQuests(null).Count ==
                                           _activeQuestGroupController.GetQuests(QuestState.Claimed).Count;

        _questManager.OnQuestProgressChange += OnQuestProgressChanged;
        _questManager.OnQuestGroupComplete += OnQuestGroupComplete;


        _isInitialized = true;
    }

    private void OnQuestGroupComplete(string questGroupId)
    {
        if (questGroupId == _activeQuestGroupController.QuestGroup.Id)
        {
            _questGroupCompleteImage.enabled = true;
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
        _questProgressText.text = $"{quest.metaData.Title} : {quest.CurrentAmount}/{quest.QuestData.Amount}";
        _questProgressSlider.DOFillAmount(quest.CurrentAmount / (float)quest.QuestData.Amount,
            1.3f);

        var wActiveQuestGroupController =
            GameInstaller.Instance.SystemLocator.QuestManager.GetActiveQuestGroupController(QuestGroupEnums
                .Daily);
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