using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BasicInGamePayer", menuName = "ScriptableObjects/Payers/BasicInGamePayer", order = 0)]
public class BasicInGamePayer : BasePayer
{
    public override void Pay(ListOfProductBlock product, Action onComplete, Action onFail)
    {
        var amount = product.priceBlock.Amount;
        if (CanPay(product.priceBlock))
        {
            GameInstaller.Instance.SystemLocator.ExchangeManager.DoExchange(CurrencyExtension.GetString(product.priceBlock.Currency), -amount);
            onComplete?.Invoke();
        }else
        {
            onFail?.Invoke();
        }
    }

    public override void Pay(PriceBlock priceBlock, Action onComplete, Action onFail)
    {
        if (CanPay(priceBlock))
        {
            GameInstaller.Instance.SystemLocator.ExchangeManager.DoExchange(CurrencyExtension.GetString(priceBlock.Currency), -priceBlock.Amount);
            onComplete?.Invoke();
        }else
        {
            onFail?.Invoke();
        }
    }

    public override bool CanPay(PriceBlock priceBlock)
    {
        return GameInstaller.Instance.SystemLocator.ExchangeManager.GetExchange(CurrencyExtension.GetString(priceBlock.Currency),0f) >= priceBlock.Amount;
    }
}
