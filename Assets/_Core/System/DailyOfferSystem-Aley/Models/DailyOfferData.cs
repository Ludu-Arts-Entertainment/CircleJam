using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "DailyOfferData", menuName = "ScriptableObjects/DailyOfferData", order = 1)]
public class DailyOfferData : ScriptableObject
{
    public int Id;
    public string Title;
    public string Description;
    public CurrencyRequirementData CurrencyPrice;
    public List<DailyOFferSubData> DailyOfferSubDatas;

    public DailyOfferData(int id, string title, string description, CurrencyRequirementData currencyPrice, List<DailyOFferSubData> dailyOfferSubDatas)
    {
        Id = id;
        Title = title;
        Description = description;
        CurrencyPrice = currencyPrice;
        DailyOfferSubDatas = dailyOfferSubDatas;
    }
}

[Serializable] 
public class  CurrencyRequirementData
{
    public string CurrencyType;
    public float MinCurrencyAmount;
    public float MaxCurrencyAmount;
} 

[Serializable]
public class DailyOFferSubData
{
    public ProductBlockType ProductBlockType;
    public ProductBlockSubType ProductBlockSubType;
    public int MinAmount;
    public int MaxAmount;
}

public class DailyOfferSaveData
{
    public readonly int DailyOfferDataId;
    public bool IsPurchased;
    public readonly ProductBlockType ProductBlockType;
    public readonly ProductBlockSubType ProductBlockSubType;
    public readonly string Id;
    public readonly int Amount;
    public readonly string PriceType;
    public readonly float PriceValue;
    public DailyOfferSaveData(int dailyOfferDataId, bool isPurchased, ProductBlockType productBlockType, ProductBlockSubType productBlockSubType, string id, int amount, string priceType, float priceValue)
    {
        DailyOfferDataId = dailyOfferDataId;
        IsPurchased = isPurchased;
        ProductBlockType = productBlockType;
        ProductBlockSubType = productBlockSubType;
        Id = id;
        Amount = amount;
        PriceType = priceType;
        PriceValue = priceValue;
    }
}
