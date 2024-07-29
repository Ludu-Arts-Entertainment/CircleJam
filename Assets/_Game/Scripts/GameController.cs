using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private EventManager _eventManager;

    private async void Awake()
    {
        await UniTask.WaitUntil(() => GameInstaller.Instance.ManagerDictionary.ContainsKey(ManagerEnums.EventManager));
        _eventManager = GameInstaller.Instance.SystemLocator.EventManager;
        _eventManager.Subscribe<Events.AskDataSyncSourceEvent>( OnAskDataSyncSourceEvent);
        _eventManager.Subscribe<Events.OnGameReadyToStart>(OnGameReadyToStart);
    }


    private void OnGameReadyToStart(Events.OnGameReadyToStart obj)
    {
        GameInstaller.Instance.SystemLocator.UIManager.Show(UITypes.IntroPanel, null);
    }

    private void OnAskDataSyncSourceEvent(Events.AskDataSyncSourceEvent obj)
    {
        DataSyncSelectPopupData dataSyncSelectPopupData = new DataSyncSelectPopupData(
            default,
            default,
            obj.LocalBasicProgressSummaryCardData,
            obj.LocalBasicProgressSummaryCardData,
            obj.OnDataSelected);
        GameInstaller.Instance.SystemLocator.UIManager.Show(UITypes.DataSyncSelectPopup, dataSyncSelectPopupData);
    }
}
