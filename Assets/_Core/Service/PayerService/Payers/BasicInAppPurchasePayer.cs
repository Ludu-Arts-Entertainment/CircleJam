using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BasicInAppPurchasePayer", menuName = "ScriptableObjects/Payers/BasicInAppPurchasePayer", order = 0)]
public class BasicInAppPurchasePayer : BasePayer
{
    public override void Pay(ListOfProductBlock product, Action onComplete, Action onFail)
    {
        onComplete?.Invoke();
    }
    public override void Pay(PriceBlock priceBlock, Action onComplete, Action onFail)
    {
        onFail?.Invoke();
    }

    public override bool CanPay(PriceBlock priceBlock)
    {
        return true;
    }
}
