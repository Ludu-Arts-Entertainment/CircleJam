using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyOfferManager : IManager
{
    private const float DAILY_OFFER_UPDATE_TIME = 24f;
    private const int MAX_DAILY_OFFER_AD_WATCH_RESET_COUNT = 3;
    private IDailyOfferProvider _dailyOfferProvider;
    private Dictionary<int, DailyOfferSaveData> DailyOfferSaveData => _dailyOfferSaveData;
    private Dictionary<int, DailyOfferSaveData> _dailyOfferSaveData = new();
    private DateTime lastUpdateTime;
    private int adWatchCount;

    public IManager CreateSelf()
    {
        return new DailyOfferManager();
    }

    public void Initialize(GameInstaller gameInstaller, Action onReady)
    {
        _dailyOfferProvider = DailyOfferProviderFactory.Create(gameInstaller.Customizer.DailyOfferProvider);
        _dailyOfferProvider.Initialize(onReady);
        LoadData();
    }
    
    private void LoadData()
    {
        _dailyOfferSaveData.Clear();
//LastDailyOfferUpdateTime
        var stateData = GameInstaller.Instance.SystemLocator.DataManager.GetData<Dictionary<string,ulong>>(GameDataType.State);
        lastUpdateTime = stateData.TryGetValue("LastDailyOfferUpdateTime", out var value) ? TimeHelper.UnixTimeStampToDateTime(value) : DateTime.Now.Date.AddSeconds(-1);
        if(lastUpdateTime != null)
        {
            var currentTime = TimeHelper.GetCurrentDateTime();
            var timeDifference = currentTime - lastUpdateTime;
            var timeDifferenceInHours = timeDifference.TotalHours;
            if(timeDifferenceInHours >= DAILY_OFFER_UPDATE_TIME)
            {
                _dailyOfferSaveData = _dailyOfferProvider.CreateNewDailyOffers();
                GameInstaller.Instance.SystemLocator.DataManager.SetData(GameDataType.DailyOfferData, _dailyOfferSaveData);
                
                DateTime midnightToday = DateTime.Now.Date.AddSeconds(-1);
                lastUpdateTime = midnightToday;
                stateData["LastDailyOfferUpdateTime"] = TimeHelper.DateTimeToUnixTimeStampInSeconds(lastUpdateTime);
                adWatchCount = 0;
                stateData["DailyOfferAdWatchCount"] = 0;
                GameInstaller.Instance.SystemLocator.DataManager.SetData(GameDataType.State, stateData);
            }
            
            else
            {
                var savedData = GameInstaller.Instance.SystemLocator.DataManager.GetData<Dictionary<int, DailyOfferSaveData>>(GameDataType.DailyOfferData);
                if(savedData != null && savedData.Count > 0)
                {
                    _dailyOfferSaveData = savedData;
                }
                else
                {
                    _dailyOfferSaveData = _dailyOfferProvider.CreateNewDailyOffers();
                    GameInstaller.Instance.SystemLocator.DataManager.SetData(GameDataType.DailyOfferData, _dailyOfferSaveData);
                    
                    DateTime midnightToday = DateTime.Now.Date.AddSeconds(-1);
                    lastUpdateTime = midnightToday;
                    stateData["LastDailyOfferUpdateTime"] = TimeHelper.DateTimeToUnixTimeStampInSeconds(lastUpdateTime);
                    GameInstaller.Instance.SystemLocator.DataManager.SetData(GameDataType.State, stateData);
                }

                LoadAdWatchCountStateData();
            }
        }
    }

    private void LoadAdWatchCountStateData()
    {
        var stateData = GameInstaller.Instance.SystemLocator.DataManager.GetData<Dictionary<string, ulong>>(GameDataType.State);
        if (stateData.TryGetValue("DailyOfferAdWatchCount", out var value))
        {
            adWatchCount = (int)value;
        }
        else
        {
            stateData.Add("DailyOfferAdWatchCount", 0);
            adWatchCount = 0;
            GameInstaller.Instance.SystemLocator.DataManager.SetData(GameDataType.State, stateData);
        }
    }

    private List<ProductBlock> givenProducts = new List<ProductBlock>();

    public void PurchaseDailyOffer(int id)
    {
        var dailyOffer = _dailyOfferSaveData[id];
        if(dailyOffer == null) return;

        if(!CanPurchaseDailyOffer(id))
        {
            return;
        }

        givenProducts.Clear();

        var productBlock = new ProductBlock();
        productBlock.type = dailyOffer.ProductBlockType;
        productBlock.subType = dailyOffer.ProductBlockSubType;
        productBlock.amount = dailyOffer.Amount;
        givenProducts.Add(productBlock);

        GiverService.Give(givenProducts, null);

        _dailyOfferSaveData[id].IsPurchased = true;
        GameInstaller.Instance.SystemLocator.DataManager.SetData(GameDataType.DailyOfferData, _dailyOfferSaveData);

       SpendCurrency(dailyOffer);
    }

    private void SpendCurrency(DailyOfferSaveData dailyOffer)
    {
        var priceBlock = new PriceBlock();
        priceBlock.Currency = CurrencyExtension.GetCurrency(dailyOffer.PriceType);
        priceBlock.Amount = dailyOffer.PriceValue;
        priceBlock.AmountString = "";

        PayerService.Pay(priceBlock, PriceType.InGame, null, null);
    }

    public void ResetReloadWithAd()
    {
         _dailyOfferSaveData = _dailyOfferProvider.CreateNewDailyOffers();
        GameInstaller.Instance.SystemLocator.DataManager.SetData(GameDataType.DailyOfferData, _dailyOfferSaveData);
       
        adWatchCount++;
        var stateData = GameInstaller.Instance.SystemLocator.DataManager.GetData<Dictionary<string, ulong>>(GameDataType.State);
        stateData["LastDailyOfferUpdateTime"] = TimeHelper.DateTimeToUnixTimeStampInSeconds(lastUpdateTime);
        stateData["DailyOfferAdWatchCount"] = (ulong)adWatchCount;
        GameInstaller.Instance.SystemLocator.DataManager.SetData(GameDataType.State, stateData);
    }

    public bool CanPurchaseDailyOffer(int id)
    {
        var dailyOffer = _dailyOfferSaveData[id];
        if(dailyOffer != null)
        {
            var priceType = dailyOffer.PriceType;
            var priceValue = dailyOffer.PriceValue;

            var currentExchange = GameInstaller.Instance.SystemLocator.ExchangeManager.GetExchange(priceType.ToString(), 0f);
            if(currentExchange >= priceValue)
                return true;
        }

        return false;
    }

    public bool CheckDailyOfferTime()
    {
        var stateData = GameInstaller.Instance.SystemLocator.DataManager.GetData<Dictionary<string, ulong>>(GameDataType.State);
        lastUpdateTime = stateData.TryGetValue("LastDailyOfferUpdateTime", out var value) ? TimeHelper.UnixTimeStampToDateTime(value) : DateTime.Now.Date.AddSeconds(-1);
        if(lastUpdateTime != null)
        {
            var currentTime = TimeHelper.GetCurrentDateTime();
            var timeDifference = currentTime - lastUpdateTime;
            var timeDifferenceInHours = timeDifference.TotalHours;
            if(timeDifferenceInHours >= DAILY_OFFER_UPDATE_TIME)
            {
                LoadData();
                return true;
            }
        }

        return false;
    }

    public TimeSpan GetRemainingTime()
    {
        return lastUpdateTime.AddHours(DAILY_OFFER_UPDATE_TIME) - TimeHelper.GetCurrentDateTime();
    }

    public DailyOfferSaveData GetDailyOffer(int id)
    {
        return _dailyOfferSaveData[id];
    }

    public Dictionary<int, DailyOfferSaveData> GetDailyOffers()
    {
        return DailyOfferSaveData;
    }

    public bool IsReady()
    {
        return _dailyOfferProvider != null;
    }

    public int GetMaxDailyOfferAdWatchCount()
    {
        return MAX_DAILY_OFFER_AD_WATCH_RESET_COUNT;
    }

    public int GetAdWatchCount()
    {
        return adWatchCount;
    }

    public bool CanWatchAd()
    {
        return adWatchCount < MAX_DAILY_OFFER_AD_WATCH_RESET_COUNT;
    }
   
}
