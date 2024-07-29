using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouletteManager : IManager
{
    private const float ROULETTE_UPDATE_TIME = 24f;
    private const int MAX_ROULETTE_AD_WATCH_COUNT = 3;
    private IRouletteProvider _rouletteProvider;
    private Dictionary<int, RouletteSaveData> RouletteSaveData => _rouletteSaveData;
    private Dictionary<int, RouletteSaveData> _rouletteSaveData = new();
    private List<int> gainableRouletteIds = new List<int>();
    private DateTime lastUpdateTime;
    private int rouletteAdWatchCount; 

    public IManager CreateSelf()
    {
        return new RouletteManager();
    }

    public void Initialize(GameInstaller gameInstaller, Action onReady)
    {
        _rouletteProvider = RouletteProviderFactory.Create(gameInstaller.Customizer.RouletteProvider);
        _rouletteProvider.Initialize(onReady);
        LoadData();
    }   

    public bool IsReady()
    {
        return _rouletteProvider != null;
    }

    private void LoadData()
    {
        _rouletteSaveData.Clear();

        lastUpdateTime = GameInstaller.Instance.SystemLocator.DataManager.GetData<DateTime>(GameDataType.LastRouletteUpdateTime);
        if(lastUpdateTime != null)
        {
            var currentTime = TimeHelper.GetCurrentDateTime();
            var timeDifference = currentTime - lastUpdateTime;
            var timeDifferenceInHours = timeDifference.TotalHours;
            if(timeDifferenceInHours >= ROULETTE_UPDATE_TIME)
            {
                _rouletteSaveData = _rouletteProvider.CreateNewRouletteDatas();
                GameInstaller.Instance.SystemLocator.DataManager.SetData(GameDataType.RouletteData, _rouletteSaveData);
                
                DateTime midnightToday = DateTime.Now.Date.AddSeconds(-1);
                lastUpdateTime = midnightToday;

                GameInstaller.Instance.SystemLocator.DataManager.SetData(GameDataType.LastRouletteUpdateTime, lastUpdateTime);
            }
            else
            {
                var savedData = GameInstaller.Instance.SystemLocator.DataManager.GetData<Dictionary<int, RouletteSaveData>>(GameDataType.RouletteData);
                if(savedData != null && savedData.Count > 0)
                {
                    _rouletteSaveData = savedData;
                }
                else
                {
                    _rouletteSaveData = _rouletteProvider.CreateNewRouletteDatas();
                    GameInstaller.Instance.SystemLocator.DataManager.SetData(GameDataType.RouletteData, _rouletteSaveData);
                    
                    DateTime midnightToday = DateTime.Now.Date.AddSeconds(-1);
                    lastUpdateTime = midnightToday;
                    
                    GameInstaller.Instance.SystemLocator.DataManager.SetData(GameDataType.LastRouletteUpdateTime, lastUpdateTime);
                }
            }
        }

        LoadGainableRouletteIds();
        LoadAdWatchCount();
    }

    private void LoadGainableRouletteIds()
    {
        gainableRouletteIds.Clear();
        foreach (var rouletteData in _rouletteSaveData)
        {
            if(!rouletteData.Value.IsGained)
            {
                gainableRouletteIds.Add(rouletteData.Key);
            }
        }
    }

    private void LoadAdWatchCount()
    {
        rouletteAdWatchCount = 0;
        foreach(var rouletteData in _rouletteSaveData)
        {
            if(rouletteData.Value.IsGained)
            {
                rouletteAdWatchCount++;
            }
        }
    }

    private List<ProductBlock> givenCards = new List<ProductBlock>();
    private List<ProductBlock> givenExchanges = new List<ProductBlock>();

    public int GetRandomGainableRouletteId()
    {
        if(gainableRouletteIds.Count <= 0) return -1;

        var randomIndex = UnityEngine.Random.Range(0, gainableRouletteIds.Count);
        var randomRouletteId = gainableRouletteIds[randomIndex];

        return randomRouletteId;
    }

    public int GetMaxRouletteAdWatchCount()
    {
        return MAX_ROULETTE_AD_WATCH_COUNT;
    }

    public int GetRouletteAdCount()
    {
        return rouletteAdWatchCount;
    }

    public bool CanWatchRouletteAd()
    {
        return rouletteAdWatchCount < MAX_ROULETTE_AD_WATCH_COUNT;
    }

    public void GainRouletteItem(int id)
    {
        var rouletteItem = _rouletteSaveData[id];
        if(rouletteItem == null) return;

        /*if(rouletteItem.ProductBlockType == ProductBlockType.Card)
        {
            givenCards.Clear();

            var productBlock = new ProductBlock();
            productBlock.type = ProductBlockType.Card;
            productBlock.subType = rouletteItem.ProductBlockSubType;
            productBlock.id = int.Parse(rouletteItem.Id);
            productBlock.amount = rouletteItem.Amount;

            givenCards.Add(productBlock);
            GiverService.Give(givenCards, null);
        }
        else if(rouletteItem.ProductBlockType == ProductBlockType.Chest)
        {
            var currentExchange = GameInstaller.Instance.SystemLocater.ExchangeManager.GetExchange(ProductBlockSubType.Trophy.ToString(), 0f);
            ChestDataService.InstantOpen(rouletteItem.Id, (int)currentExchange);
        }*/
        if(rouletteItem.ProductBlock.type == ProductBlockType.Currency)
        {
            givenExchanges.Clear();

            var productBlock = new ProductBlock();
            productBlock.type = ProductBlockType.Currency;
            productBlock.subType = rouletteItem.ProductBlock.subType;
            productBlock.amount = rouletteItem.ProductBlock.amount;
            givenExchanges.Add(productBlock);

            GiverService.Give(givenExchanges, null);
        }

        _rouletteSaveData[id].IsGained = true;
        gainableRouletteIds.Remove(id);
        rouletteAdWatchCount++;

        GameInstaller.Instance.SystemLocator.DataManager.SetData(GameDataType.RouletteData, _rouletteSaveData);
        GameInstaller.Instance.SystemLocator.EventManager.Trigger(new Events.OnRouletteItemGain(id));
       
    }

    public bool CheckRouletteTime()
    {
        lastUpdateTime = GameInstaller.Instance.SystemLocator.DataManager.GetData<DateTime>(GameDataType.LastRouletteUpdateTime);
        if(lastUpdateTime != null)
        {
            var currentTime = TimeHelper.GetCurrentDateTime();
            var timeDifference = currentTime - lastUpdateTime;
            var timeDifferenceInHours = timeDifference.TotalHours;
            if(timeDifferenceInHours >= ROULETTE_UPDATE_TIME)
            {
                LoadData();
                return true;
            }
        }

        return false;
    }

    public TimeSpan GetRemainingTime()
    {
        return lastUpdateTime.AddHours(ROULETTE_UPDATE_TIME) - TimeHelper.GetCurrentDateTime();
    }

    public RouletteSaveData GetRouletteItem(int id)
    {
        return _rouletteSaveData[id];
    }

    public Dictionary<int, RouletteSaveData> GetRouletteItems()
    {
        return RouletteSaveData;
    }
}

public partial class Events
{
    public class OnRouletteItemGain : IEvent
    {
        public int Id { get; }
        public OnRouletteItemGain(int id)
        {
            Id = id;
        }
    }
}
