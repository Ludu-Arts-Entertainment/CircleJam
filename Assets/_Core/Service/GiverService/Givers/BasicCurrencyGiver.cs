using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BasicCurrencyGiver", menuName = "ScriptableObjects/Givers/BasicCurrencyGiver", order = 0)]
public class BasicCurrencyGiver : BaseGiver
{
    private Dictionary<ProductBlockSubType,Currency> currencyTypes = new ()
    {
        {ProductBlockSubType.Gem,Currency.Gem},
        {ProductBlockSubType.Coin,Currency.Coin}
    };
    public override void Give(ProductBlock productBlock, Action onComplete, Action onFail)
    {
        GameInstaller.Instance.SystemLocator.ExchangeManager.DoExchange(CurrencyExtension.GetString(currencyTypes[productBlock.subType]), productBlock.amount);
        onComplete?.Invoke();
    }
}