using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DailyLoginPopup : PopupBase
{
    [SerializeField] private BasicDailyRewardElement[] dailyRewardElements;
    [SerializeField] private TMP_Text nextRewardText;
    private string _nextRewardTextFormat = "Next Reward in \n{0}:{1} minutes";
    private BasicDailyRewardElement activeElement;
    private bool isClaimable;
    public override void Show(IBaseUIData data)
    {
        isClaimable = GameInstaller.Instance.SystemLocator.DailyLoginManager.IsClaimable();
        Prepare();
        GameInstaller.Instance.SystemLocator.DailyLoginManager.OnClaimed+= OnClaimed;
        base.Show(data);
    }
    private void FixedUpdate()
    {
        if (isClaimable) return;
        isClaimable = GameInstaller.Instance.SystemLocator.DailyLoginManager.IsClaimable();
        var drm = GameInstaller.Instance.SystemLocator.DailyLoginManager;
        var nextTime = drm.GetNextClaimTime();
        nextRewardText.text = nextTime.TotalSeconds>0 ? string.Format(_nextRewardTextFormat, nextTime.Hours, nextTime.Minutes) : string.Empty;
        if (!isClaimable) return;
        foreach (var dre in dailyRewardElements)
        {
            dre.Dispose();
        }
        Prepare();
    }

    public override void Hide()
    {
        foreach (var dre in dailyRewardElements)
        {
            dre.Dispose();
        }
        base.Hide();
    }
    private void Prepare()
    {
        for (var i = 0; i < dailyRewardElements.Length; i++)
        {
            var drm = GameInstaller.Instance.SystemLocator.DailyLoginManager;
            var drd = drm.GetDailyLoginReward(i);
            var claimStatus = drm.GetDailyLoginRewardStatus(i);
            switch (claimStatus)
            {
                case DailyLoginRewardStatus.Claimable:
                    activeElement = dailyRewardElements[i];
                    dailyRewardElements[i].Initialize(drd, drm.Claim);
                    break;
                case DailyLoginRewardStatus.Claimed:
                    dailyRewardElements[i].Initialize(drd, null, true);
                    break;
                case DailyLoginRewardStatus.UnClaimable:
                default:
                    dailyRewardElements[i].Initialize(drd, null, false);
                    break;
            }
        }
    }
    private void OnClaimed(int obj)
    {
        activeElement.Dispose();
        activeElement.Claim();
        isClaimable = GameInstaller.Instance.SystemLocator.DailyLoginManager.IsClaimable();
    }
}
public partial class UITypes
{
    public const string DailyLoginPopup = "DailyLoginPopup";
}
