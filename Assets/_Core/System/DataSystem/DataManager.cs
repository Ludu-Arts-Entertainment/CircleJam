using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using PlayFab;
using UnityEngine.Device;
using UnityEngine.Networking;

public class DataManager : IManager
{
    private IData _data;
    private IDataProvider _dataProvider;
    private IDataProvider _playFabDataProvider;

    public IManager CreateSelf()
    {
        return new DataManager();
    }

    public async void Initialize(GameInstaller gameInstaller, Action onReady)
    {
        _dataProvider = DataProviderFactory.Create(gameInstaller.Customizer.DataProvider);
        _dataProvider.Initialize(this);
        _data = await _dataProvider.LoadAll(_data);
        
        onReady.Invoke();

        // _dataProvider.Load(_data, (data) =>
        // {
        //     _data = data;
        //     onReady.Invoke();
        // }, typeof(GameData));

        // UniTask.WhenAll(_playFabDataProvider.LoadAll(_data));
        //
#if PlayFabSdk_Enabled
        _playFabDataProvider = DataProviderFactory.Create(DataProviderEnums.PlayFabDataProvider);
        _playFabDataProvider.Initialize(this);
#endif
    }

    public bool IsReady()
    {
        return _data != null;
    }

    public T GetData<T>(GameDataType key)
    {
        return _data.GetData<T>(key);
    }
    
    public object GetData(GameDataType key)
    {
        return _data.GetData(key);
    }

    public void SetData<T>(GameDataType key, T value)
    {
        _data.SetData(key, value);
    }

    public void SetData(Dictionary<string, string> data)
    {
        _data.SetData(data);
    }

    public void SaveData()
    {
        UpdateDataHistory();
        _dataProvider.SaveAll(_data);
    }

    public async UniTask SaveRemoteData()
    {
#if PlayFabSdk_Enabled
        await ((PlayFabDataProvider) _playFabDataProvider).SaveRemote(_data);
#endif
    }

    public void UpdateDataHistory()
    {
        var dataHistory = GetData<GameDataHistory>(GameDataType.GameDataHistory);

        if (!dataHistory.Records.TryGetValue(SystemInfo.deviceUniqueIdentifier, out uint currentVersion))
        {
            dataHistory.Records.Add(SystemInfo.deviceUniqueIdentifier, 1);
        }
        else
        {
            dataHistory.Records[SystemInfo.deviceUniqueIdentifier] = currentVersion + 1;
        }

    }

    public HashSet<GameDataType> GetDirtyDataTypes()
    {
        return _data.GetDirtyDataTypes();
    }

    public IData GetDataObject()
    {
        return _data;
    }

#if PlayFabSdk_Enabled
    public IDataProvider GetPlayFabDataProvider()
    {
        return _playFabDataProvider;
    }
#endif
}