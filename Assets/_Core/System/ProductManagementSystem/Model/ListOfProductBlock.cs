using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

[System.Serializable]
public class ListOfProductBlock
{
    public string id;
    public string name;
    public string description;
    public bool isAvailable;
    public PriceType priceType;
    public PriceBlock priceBlock;
#if IAPManager_Enabled
    public string googlePlayId;
    public string appleStoreId;
#endif
    public List<ProductBlock> listOfProductBlocks;
    public Sprite icon;
    public ProductType productType;
    public int availableCount;
    public float availableTime;

    internal string GetStoreID()
    {
#if UNITY_IOS && IAPManager_Enabled
        return appleStoreId;
#elif UNITY_ANDROID && IAPManager_Enabled // 
        return googlePlayId;
#else
        return "";
#endif
    }
}


public enum PriceType
{
    IAP,
    InGame,
    Rewarded
}

[System.Serializable]
public class PriceBlock
{
    public float Amount;
    public string AmountString;
    public Currency Currency;
    public int id;
}

public enum Currency
{
    Coin = 0,
    Gem = 1,
    Dollar = 2,
    Rewarded = 3,
    Energy = 4,
}
public class CurrencyExtension
{
    public static string GetString(Currency currency)
    {
        return currency.ToString();
    }
    
    public static Currency GetCurrency(string currency)
    {
        return Enum.Parse<Currency>(currency);
    }
}