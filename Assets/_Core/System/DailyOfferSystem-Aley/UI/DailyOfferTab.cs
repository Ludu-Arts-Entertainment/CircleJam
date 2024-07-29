using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyOfferTab : MonoBehaviour
{
    [SerializeField] private GameObject normalBG, doneObject;
    [SerializeField] private TextMeshProUGUI titleText, amountText, priceText;
    [SerializeField] private Image tabBGImage, iconBGImage, iconImage, priceImage;
    [SerializeField] private Color normalColor, freeColor, inActiveColor;

    private Button button;
    private Button Button => button ??= GetComponent<Button>();

    private DailyOfferSaveData _dailyOfferData;
    private bool _isButtonActive;
    private void OnEnable() 
    {
        Button.onClick.AddListener(OnButtonClicked);
    }

    private void OnDisable() 
    {
        Button.onClick.RemoveListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        if(GameInstaller.Instance.SystemLocator.DailyOfferManager.CanPurchaseDailyOffer(_dailyOfferData.DailyOfferDataId))
        {
            GameInstaller.Instance.SystemLocator.DailyOfferManager.PurchaseDailyOffer(_dailyOfferData.DailyOfferDataId);
            SetPurchased(_dailyOfferData);
        }
        else
        {
            Debug.Log($"Not Enough {_dailyOfferData.PriceValue}!");
        }
    }

    private void SetPurchased(DailyOfferSaveData data)
    {
        _dailyOfferData = data;
        Button.enabled = _isButtonActive && !_dailyOfferData.IsPurchased;
        if(_dailyOfferData.IsPurchased) tabBGImage.color = inActiveColor;
        doneObject.SetActive(_dailyOfferData.IsPurchased);
    }

    public void Load(DailyOfferSaveData data, bool isButtonActive = true)
    {
        _dailyOfferData = data;
        _isButtonActive = isButtonActive;

        if(_dailyOfferData == null) return;

        iconImage.sprite = GameInstaller.Instance.SystemLocator.ProductManager.GetProductIcon(_dailyOfferData.ProductBlockType, _dailyOfferData.ProductBlockSubType);
        normalBG.SetActive(true);
        iconBGImage.gameObject.SetActive(false);
        amountText.text = $"{_dailyOfferData.Amount}";
        titleText.text = _dailyOfferData.ProductBlockSubType.ToString();

        var priceValue = _dailyOfferData.PriceValue;
        if(priceValue <= 0)
        {
            tabBGImage.color = freeColor;
            priceImage.gameObject.SetActive(false);
            priceText.text = "FREE!";
        }
        else
        {
            var priceType = _dailyOfferData.PriceType;
            tabBGImage.color = normalColor;
            priceImage.gameObject.SetActive(true);
            //priceImage.sprite = GameInstaller.Instance.SystemLocator.ProductManager.GetProductIcon(ProductBlockType.Currency, _dailyOfferData.PriceType);
            priceText.text = $"{_dailyOfferData.PriceValue}";
        }
    }

}
