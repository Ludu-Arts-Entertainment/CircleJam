using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class WatchToEarnRewardElement : MonoBehaviour
{
    [BoxGroup("UI Elements"), SerializeField] private TMPro.TextMeshProUGUI TitleText;
    [BoxGroup("UI Elements"), SerializeField] private TMPro.TextMeshProUGUI OrderText;
    [BoxGroup("UI Elements"), SerializeField] private Image RewardIcon;
    
    [BoxGroup("Status/Icons"),SerializeField] private GameObject LockedIcon;
    [BoxGroup("Status/Icons"),SerializeField] private GameObject ClaimedIcon;
    
    [BoxGroup("Status/Backgrounds"),SerializeField] private GameObject ClaimableBackground;
    [BoxGroup("Status/Backgrounds"),SerializeField] private GameObject ClaimedBackground;
    [BoxGroup("Status/Backgrounds"),SerializeField] private GameObject UnClaimedBackground;
    
    private WatchToEarnRewardStatus _status;
    
    public void SetData(WatchToEarnRewardStatus status, WatchToEarnReward reward, int order)
    {
        ChangeStatus(status);
        TitleText.text = reward.metaData.Title;
        OrderText.text = order.ToString();
    }
    
    
    private void ChangeStatus(WatchToEarnRewardStatus status)
    {
        _status = status;
        SetStatus();
    }
    private void SetStatus()
    {
        switch (_status)
        {
            case WatchToEarnRewardStatus.Claimed:
                LockedIcon.SetActive(false);
                ClaimedIcon.SetActive(true);
                ClaimableBackground.SetActive(false);
                ClaimedBackground.SetActive(true);
                UnClaimedBackground.SetActive(true);
                break;
            case WatchToEarnRewardStatus.Claimable:
                LockedIcon.SetActive(false);
                ClaimedIcon.SetActive(false);
                ClaimableBackground.SetActive(true);
                ClaimedBackground.SetActive(false);
                UnClaimedBackground.SetActive(false);
                break;
            case WatchToEarnRewardStatus.UnClaimable:
                LockedIcon.SetActive(true);
                ClaimedIcon.SetActive(false);
                ClaimableBackground.SetActive(false);
                ClaimedBackground.SetActive(false);
                UnClaimedBackground.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
