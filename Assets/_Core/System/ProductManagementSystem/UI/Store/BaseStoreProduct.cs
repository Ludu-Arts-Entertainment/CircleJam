using System;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseStoreProduct : MonoBehaviour
{
    [SerializeField] private TMP_Text NameText;
    [SerializeField] private TMP_Text DescriptionText;
    [SerializeField] private TMP_Text PriceText;
    [SerializeField] private Image Icon;
    [SerializeField] private Button PurchaseButton;
    private const string TimeFormat = @"hh\:mm\:ss";

    public delegate void PurchaseEvent(ListOfProductBlock product, Action OnComplete, Action OnFailed = null);

    public event PurchaseEvent OnPurchase;
    private ListOfProductBlock Model;
    
    public virtual void Initialize(ListOfProductBlock model)
    {
        Model = model;
        NameText.text = model.name;
        DescriptionText.text = model.description;
        Icon.sprite = model.icon;
        Check();
    }

    private void OnEnable()
    {
        PurchaseButton.onClick.AddListener(Purchase);
    }

    private void OnDisable()
    {
        PurchaseButton.onClick.RemoveListener(Purchase);
    }

    private void Purchase()
    {
        PurchaseButton.interactable = false;
        OnPurchase?.Invoke(Model, HandlePurchaseComplete,HandlePurchaseComplete);
    }

    private void Check()
    {
        var purchaseResult = GameInstaller.Instance.SystemLocator.ProductManager.CanPurchase(Model.id);
        switch (purchaseResult.Item1)
        {
            case PurchaseCheckResult.AlreadyPurchased:
                PriceText.text = "Purchased";
                PurchaseButton.interactable = false;
                break;
            case PurchaseCheckResult.NotAvailable:
                PriceText.text = "Not Available";
                PurchaseButton.interactable = false;
                break;
            case PurchaseCheckResult.TimeLimit:
                PurchaseButton.interactable = false;
                PriceText.text = $"{purchaseResult.Item2.ToString(TimeFormat)}";
                if (CoroutineController.IsCoroutineRunning(timerRoutineName))
                {
                    CoroutineController.StopCoroutine(timerRoutineName);
                }

                CoroutineController.DoAfterGivenTime(1, Check, timerRoutineName);
                return;
            default:
            {
                if (Model.priceBlock.Amount <= 0 || Model.priceBlock.Currency != Currency.Dollar)
                {
                    PriceText.text =
                        $"<sprite=\"Currencies\" name=\"{Model.priceBlock.Currency.ToString()}\">" +
                        Model.priceBlock.Amount.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    PriceText.text =
                        Model.priceBlock.AmountString.ToString(CultureInfo.InvariantCulture)+" "+
                        Model.priceBlock.Amount.ToString(CultureInfo.InvariantCulture);
                }
                PurchaseButton.interactable = Model.isAvailable;
                break;
            }
        }
        if (CoroutineController.IsCoroutineRunning(timerRoutineName))
        {
            CoroutineController.StopCoroutine(timerRoutineName);
        }
    }

    private void HandlePurchaseComplete()
    {
        Check();
    }

    private string timerRoutineName => $"TimerRoutine+{GetInstanceID()}";
}