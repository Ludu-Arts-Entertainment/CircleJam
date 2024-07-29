using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BasicDailyRewardElement : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Image[] iconImage;
    [SerializeField] private TMP_Text[] amountText;
    [SerializeField] private Button claimButton;
    [SerializeField] private GameObject claimedBanner;
    private Action _onClick;
    
    public void Initialize(DailyLoginReward reward, Action onClick, bool isClaimed=false)
    {
        titleText.text = reward.metaData.Title;
        _onClick = onClick;
        var queue = 0;
        foreach (var pb in reward.ProductBlocks)
        {
            iconImage[queue].gameObject.SetActive(true);
            amountText[queue].gameObject.SetActive(true);
            iconImage[queue].sprite =
                GameInstaller.Instance.SystemLocator.ProductManager.GetProductIcon(pb.type, pb.subType);
            amountText[queue].text = pb.amount.ToString(CultureInfo.InvariantCulture);
            queue++;
            if (queue >= iconImage.Length)
            {
                break;
            }
        }

        if (queue < iconImage.Length)
        {
            for (int i = queue; i < iconImage.Length; i++)
            {
                iconImage[i].gameObject.SetActive(false);
                amountText[i].gameObject.SetActive(false);
            }
        }
        
        claimButton.interactable = onClick != null;
        claimButton.onClick.AddListener(OnClick);
        claimedBanner.SetActive(isClaimed);
    }

    public void Dispose()
    {
        _onClick = null;
        claimButton.onClick.RemoveListener(OnClick);
    }
    
    private void OnClick()
    {
        _onClick?.Invoke();
    }

    public void Claim()
    {
        claimedBanner.SetActive(true);
    }
}
