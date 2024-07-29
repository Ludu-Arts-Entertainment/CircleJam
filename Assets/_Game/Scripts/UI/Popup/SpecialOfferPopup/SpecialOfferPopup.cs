using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpecialOfferPopup : PopupBase
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Button purchaseButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Image iconImage;
    
    private SpecialOfferData _specialOfferData;
    private ListOfProductBlock _productBlock;
    public override void Show(IBaseUIData data)
    {
        _specialOfferData = ((SpecialOfferPopupData) data).SpecialOfferData;
        _productBlock = GameInstaller.Instance.SystemLocator.ProductManager.GetProductBlock(_specialOfferData.ListOfProductBlockId, out var value);
        ValueSetter();
        closeButton.onClick.AddListener(OnClick_Close);
        purchaseButton.onClick.AddListener(OnClick_Purchase);
        base.Show(data);
    }

    private void OnClick_Purchase()
    {
        GameInstaller.Instance.SystemLocator.ProductManager.Purchase(_productBlock.id, OnPurchaseComplete, OnPurchaseFailed, _specialOfferData.Id);
    }

    private void OnPurchaseFailed()
    {
        GameInstaller.Instance.SystemLocator.UIManager.Hide(UITypes.SpecialOfferPopup);
    }

    private void OnPurchaseComplete()
    {
        GameInstaller.Instance.SystemLocator.UIManager.Hide(UITypes.SpecialOfferPopup);
    }

    private void OnClick_Close()
    {
        GameInstaller.Instance.SystemLocator.UIManager.Hide(UITypes.SpecialOfferPopup);
    }

    private void ValueSetter()
    {
        titleText.text = _specialOfferData.Title;
        descriptionText.text = _specialOfferData.Description;
        if (_productBlock.priceBlock.Amount <= 0 || _productBlock.priceBlock.Currency != Currency.Dollar)
        {
            priceText.text =
                $"<sprite=\"Currencies\" name=\"{_productBlock.priceBlock.Currency.ToString()}\">" +
                _productBlock.priceBlock.Amount.ToString(CultureInfo.InvariantCulture);
        }
        else
        {
            priceText.text =
                _productBlock.priceBlock.AmountString.ToString(CultureInfo.InvariantCulture)+" "+
                _productBlock.priceBlock.Amount.ToString(CultureInfo.InvariantCulture);
        }
        iconImage.sprite = _specialOfferData.Icon;
    }
}
public class SpecialOfferPopupData : IBaseUIData
{
    public SpecialOfferData SpecialOfferData;
    public SpecialOfferPopupData(SpecialOfferData specialOfferData)
    {
        SpecialOfferData = specialOfferData;
    }
}
