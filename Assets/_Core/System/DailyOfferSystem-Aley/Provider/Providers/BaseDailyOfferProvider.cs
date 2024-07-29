using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseDailyOfferProvider : IDailyOfferProvider
{
    private Dictionary<int, DailyOfferSaveData> _createdDailyOffers = new();
    private List<int> _createdCardIds = new List<int>();
    private DailyOfferContainer _dailyOfferContainer;
    public IDailyOfferProvider CreateSelf()
    {
        return new BaseDailyOfferProvider();
    }

    public void Initialize(Action onReady)
    {
        _dailyOfferContainer = Resources.Load<DailyOfferContainer>("DailyOfferContainer");
        onReady?.Invoke();
    }

    public Dictionary<int, DailyOfferSaveData> CreateNewDailyOffers()
    {
        _createdDailyOffers.Clear();
        _createdCardIds.Clear();

        var dailyOfferDatas = new List<DailyOfferData>();
        foreach (var data in _dailyOfferContainer.DailyOfferDatas)
        {
            var subDatas = data.DailyOfferSubDatas;
            var newSubDatas = new List<DailyOFferSubData>();
            foreach(var sub in subDatas)
            {
                newSubDatas.Add(sub);
            }

            var newDailyOfferData = new DailyOfferData(data.Id, data.Title, data.Description, data.CurrencyPrice, newSubDatas);
            dailyOfferDatas.Add(newDailyOfferData);
        }

        foreach (var dailyOfferData in dailyOfferDatas)
        {
            var dailyOfferSubData = dailyOfferData.DailyOfferSubDatas[UnityEngine.Random.Range(0, dailyOfferData.DailyOfferSubDatas.Count)];

            var currencyAmount = UnityEngine.Random.Range(dailyOfferSubData.MinAmount, dailyOfferSubData.MaxAmount);
            _createdDailyOffers.Add(dailyOfferData.Id, 
            new DailyOfferSaveData
            (
                dailyOfferDataId: dailyOfferData.Id,
                isPurchased: false,
                productBlockType: dailyOfferSubData.ProductBlockType,
                productBlockSubType: dailyOfferSubData.ProductBlockSubType, 
                id: dailyOfferSubData.ProductBlockSubType.ToString(), 
                amount: currencyAmount,
                priceType: dailyOfferData.CurrencyPrice.CurrencyType,
                priceValue: UnityEngine.Random.Range((int)dailyOfferData.CurrencyPrice.MinCurrencyAmount, (int)dailyOfferData.CurrencyPrice.MaxCurrencyAmount)
            ));
        }
        return _createdDailyOffers;
    }

}
