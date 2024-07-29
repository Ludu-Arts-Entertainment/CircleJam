using UnityEngine;

[CreateAssetMenu(fileName = "BasicGemRequirement", menuName = "ScriptableObjects/Requirements/BasicGemRequirement")]
public class BasicGemRequirement : BaseRequirement
{
    private Currency currencyType => Currency.Gem;
    public override bool Check(float minValue, float maxValue)
    {
        float currentAmount = GameInstaller.Instance.SystemLocator.ExchangeManager.GetExchange(CurrencyExtension.GetString(currencyType),0f);
        return currentAmount >= minValue && currentAmount <= maxValue;
    }
}