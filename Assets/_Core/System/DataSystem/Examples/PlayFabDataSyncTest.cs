#if PlayFabSdk_Enabled

using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class PlayFabDataSyncTest : MonoBehaviour
{
    [SerializeField] List<GameDataType> _dirtyDataList = new List<GameDataType>();
    [SerializeField] GameDataType _gameDataType;
    
    private DataManager _dataManager;
    private PlayFabDataProvider _playFabDataProvider;
    private IData _data;
    
    private void Start()
    {
        _dataManager = GameInstaller.Instance.SystemLocator.DataManager;
        _data = _dataManager.GetDataObject();
        _playFabDataProvider = _dataManager.GetPlayFabDataProvider() as PlayFabDataProvider;
    }
    
    [Button("Add GameDataType to DirtyDataList")]
    public void MarkedAsDirtyData()
    {
        if (!_dirtyDataList.Contains(_gameDataType))
        {
            _dirtyDataList.Add(_gameDataType);    
        }
        else
        {
            Debug.LogError($"_dirtyDataList already contains {_gameDataType.ToString()}");
        }
    }

    [Button]
    public async void GetDirtyData()
    {
        _playFabDataProvider ??= _dataManager.GetPlayFabDataProvider() as PlayFabDataProvider;
        await _playFabDataProvider.GetFromPlayFab(_dirtyDataList.ToHashSet());
    }
    
    [Button]
    public async void SendDirtyData()
    {
        _playFabDataProvider ??= _dataManager.GetPlayFabDataProvider() as PlayFabDataProvider;
        await _playFabDataProvider.SendToPlayFab(_data, _dirtyDataList.ToHashSet());
    }
    
    [Button]
    public async void MarkAllDataAsDirty()
    {
        var typelist = Enum.GetValues(typeof(GameDataType))
            .Cast<GameDataType>()
            .ToList();
        
        _dirtyDataList.Clear();
        _dirtyDataList.AddRange(typelist);
    }
}
#endif
