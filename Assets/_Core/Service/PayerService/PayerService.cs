using System;
using System.Collections.Generic;
using UnityEngine;

public static class PayerService
{
    private static readonly Dictionary<PriceType, IPayer> Payers = new Dictionary<PriceType, IPayer>();
    public static void Pay(ListOfProductBlock productBlock, Action OnComplete, Action OnFail)
    {
        var payer = GetPayer(productBlock.priceType);
        payer.Pay(productBlock, OnComplete, OnFail);
    }
    public static void Pay(PriceBlock priceBlock, PriceType priceType, Action OnComplete, Action OnFail)
    {
        var payer = GetPayer(priceType);
        payer.Pay(priceBlock, OnComplete, OnFail);
    }
    public static bool CanPay(PriceBlock priceBlock, PriceType priceType)
    {
        var payer = GetPayer(priceType);
        return payer.CanPay(priceBlock);
    }
    private static IPayer GetPayer(PriceType priceType)
    {
        if (Payers.TryGetValue(priceType, out var payer))
        {
            return payer;
        }
        payer = GameInstaller.Instance.Payers.Find(x => x.priceType == priceType).payer;
        if (payer == null)
        {
            Debug.Log("Payer not found for " + priceType);
            return null;
        }
        Payers.Add(priceType, payer);
        return payer;
    }
}
[Serializable]
public class PriceTypeGiverTuple
{
    public PriceType priceType;
    public BasePayer payer;
}