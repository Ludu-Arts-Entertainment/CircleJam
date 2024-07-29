using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WatchToEarnPopup : PopupBase
{
    [SerializeField] private List<WatchToEarnRewardElement> rewardElements;
    #region Managers
    private WatchToEarnManager _watchToEarnManager;
    #endregion
    [SerializeField] private Button _watchButton;
    [SerializeField] private TMP_Text _timerText;
    private string timerTextFormat = "Reset in \n{0}h {1}m {2}s";
    private int _remainingTime;
    private CancellationTokenSource _cts;
    public override void Show(IBaseUIData data)
    {
        _watchToEarnManager??=GameInstaller.Instance.SystemLocator.WatchToEarnManager;
        OnClaimed(0);
        _watchToEarnManager.OnClaimed += OnClaimed;
        _watchToEarnManager.OnRemained += OnRemained;
        _cts = new CancellationTokenSource();
        _watchButton.onClick.AddListener(Watch);
        base.InputBlocker(true);
        base.Show(data);
    }

    private void Watch()
    {
        PayerService.Pay(new PriceBlock(){Amount = 1, AmountString = "", Currency = Currency.Rewarded}, priceType: PriceType.Rewarded,() =>
        {
            _watchToEarnManager.Claim();
        },null);
    }
    public override void Hide()
    {
        _cts?.Cancel();
        _watchToEarnManager.OnClaimed -= OnClaimed;
        _watchToEarnManager.OnRemained -= OnRemained;
        _watchButton.onClick.RemoveListener(Watch);
        base.InputBlocker(false);
        base.Hide();
    }
    public void Close()
    {
        GameInstaller.Instance.SystemLocator.UIManager.Hide(UITypes.WatchToEarnPopup);
    }
    private void OnClaimed(int obj)
    {
        _watchButton.interactable = false || _watchToEarnManager.IsClaimable();
        Set();
    }
    private void OnRemained()
    {
        _watchButton.interactable = false || _watchToEarnManager.IsClaimable();
        Set();
    }
    private void Set()
    {
        var rewards = _watchToEarnManager.GetWatchToEarnRewards();
        for (int i = 0; i < rewards.Count; i++)
        {
            rewardElements[i].SetData(_watchToEarnManager.GetWatchToEarnRewardStatus(i),rewards[i],i+1);
        }
    }
    private void FixedUpdate()
    {
        _remainingTime = _watchToEarnManager.GetRemainingTime();
        _timerText.text = string.Format(timerTextFormat, _remainingTime/60/60, (_remainingTime/60)%60, _remainingTime%60);
    }
}
public partial class UITypes
{
    public const string WatchToEarnPopup = "WatchToEarnPopup";
}
