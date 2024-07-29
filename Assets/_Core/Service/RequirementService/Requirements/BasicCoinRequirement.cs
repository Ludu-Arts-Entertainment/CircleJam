using UnityEngine;

[CreateAssetMenu(fileName = "BasicCoinRequirement", menuName = "ScriptableObjects/Requirements/BasicCoinRequirement", order = 0)]
public class BasicCoinRequirement : BaseRequirement
{
    private Currency currencyType => Currency.Coin;
    public override bool Check(float minValue, float maxValue)
    {
        float currentAmount = GameInstaller.Instance.SystemLocator.ExchangeManager.GetExchange(CurrencyExtension.GetString(currencyType),0f);
        return currentAmount >= minValue && currentAmount <= maxValue;
    }
}