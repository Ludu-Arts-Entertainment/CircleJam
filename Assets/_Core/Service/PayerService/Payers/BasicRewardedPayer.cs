using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BasicRewardedPayer", menuName = "ScriptableObjects/Payers/BasicRewardedPayer", order = 0)]
public class BasicRewardedPayer : BasePayer
{
    public override void Pay(ListOfProductBlock productBlock, Action onComplete, Action onFail)
    {
        PayerService.Pay(productBlock.priceBlock, PriceType.InGame, onComplete, ()=>
        {
            GameInstaller.Instance.SystemLocator.AdManager.ShowRewarded(()=>
            {
                onComplete?.Invoke();
            }, onFail);
        });
    }

    public override void Pay(PriceBlock priceBlock, Action onComplete, Action onFail)
    {
        PayerService.Pay(priceBlock, PriceType.InGame, onComplete, ()=>
        {
            GameInstaller.Instance.SystemLocator.AdManager.ShowRewarded(()=>
            {
                onComplete?.Invoke();
            }, onFail);
        });
    }

    public override bool CanPay(PriceBlock priceBlock)
    {
        return true;
        //return GameInstaller.Instance.SystemLocator.ExchangeManager.GetExchange(currencyTypes[priceBlock.Currency],new BigInteger(0)) >= (BigInteger)priceBlock.Amount;
    }
}
