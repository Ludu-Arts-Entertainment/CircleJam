using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BasicSpecialOfferProvider : ISpecialOfferProvider
{
    private SpecialOfferDataContainer _specialOfferDataContainer;
    private SpecialOfferTrackListener _specialOfferTrackListener;
    public ISpecialOfferProvider CreateSelf()
    {
        return new BasicSpecialOfferProvider();
    }
    public void Initialize(Action onReady)
    {
        _specialOfferDataContainer = Resources.Load<SpecialOfferDataContainer>("SpecialOfferDataContainer");
        GameInstaller.Instance.SystemLocator.EventManager.Subscribe<Events.OnGameReadyToStart>(OnGameReadyToStart);
        _specialOfferTrackListener= new SpecialOfferTrackListener();
        _specialOfferTrackListener.Initialize();
        onReady?.Invoke();
    }
    
    private void OnGameReadyToStart(Events.OnGameReadyToStart obj)
    {
        GameInstaller.Instance.SystemLocator.EventManager.Unsubscribe<Events.OnGameReadyToStart>(OnGameReadyToStart);
        var list = FilteredRequirementsCheck();
        foreach (var sod in list)
        {
            Trigger(sod);
        }
    }
    public void Trigger(SpecialOfferData specialOfferData)
    {
        GameInstaller.Instance.SystemLocator.EventManager.Trigger(new Events.OnChangeActiveSpecialOfferData(specialOfferData));
    }
    public List<SpecialOfferData> FilteredRequirementsCheck()
    {
        var sodList = _specialOfferDataContainer?.ListOfSpecialOfferData.Where(aSpecialOfferData => aSpecialOfferData.RequirementCheck()).ToList();
        var storeTransactionHistory = GameInstaller.Instance.SystemLocator.DataManager
            .GetData<List<Dictionary<string, List<StoreTransaction>>>>(GameDataType.StoreTransactionHistory);
        
        foreach (var sod in new List<SpecialOfferData>(sodList))
        {
            var tempProductId = sod.ListOfProductBlockId + (string.IsNullOrEmpty(sod.Id) ? "" : "+" + sod.Id);
            storeTransactionHistory.ForEach(aDictionary =>
            {
                if (aDictionary.ContainsKey(tempProductId))
                {
                    sodList.Remove(sod);
                }
            });
        }
        return sodList;
    }
}
public partial class UITypes
{
    public const string SpecialOfferPopup = "SpecialOfferPopup";
}
public partial class Events
{
    public struct OnChangeActiveSpecialOfferData : IEvent
    {
        public SpecialOfferData SpecialOfferData;
        public OnChangeActiveSpecialOfferData(SpecialOfferData specialOfferData)
        {
            SpecialOfferData = specialOfferData;
        }
    }
}