using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using SystemInfo = UnityEngine.Device.SystemInfo;

public class PlayFabDataProvider : IDataProvider
{
    private DataManager _dataManager;
    // private Dictionary<string, string> _nonsharedRawBatchData;
    // private Dictionary<string, string> _sharedRawBatchData;
    // private List<Dictionary<string, string>> _sharedDataBatches;
    // private List<Dictionary<string, string>> _nonsharedDataBatches;

    private PlayFabDataBatcher _dataBatcher;
    private readonly int _batchSize = 10;

    public void Initialize(DataManager dataManager)
    {
        _dataManager = dataManager;
        _dataBatcher = new PlayFabDataBatcher(_batchSize);
        // _nonsharedRawBatchData = new Dictionary<string, string>(GameData.GameDataBiMap.Count() + GameData.GameDataBiMapShared.Count());
        // _sharedRawBatchData = new Dictionary<string, string>(GameData.GameDataBiMap.Count() + GameData.GameDataBiMapShared.Count());
        // _nonsharedDataBatches = new List<Dictionary<string, string>>((GameData.GameDataBiMapShared.Count() / _batchSize) + 1);
        // _sharedDataBatches = new List<Dictionary<string, string>>((GameData.GameDataBiMapShared.Count() / _batchSize) + 1);
    }

    public IDataProvider CreateSelf()
    {
        return new PlayFabDataProvider();
    }

    /// <summary>
    /// This method is used to load data from PlayFab according to the requestedDataTypes.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="requestedDataTypes"></param>
    public async UniTask Load(IData data, HashSet<GameDataType> requestedDataTypes)
    {
        var receivedData = await GetFromPlayFab(requestedDataTypes);
        data.SetData(receivedData);
    }

    /// <summary>
    /// This method is used to load data from PlayFab according to the dirtyDataTypes.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public async UniTask<IData> Load(IData data)
    {
        var receivedData = await GetFromPlayFab(data.GetDirtyDataTypes());
        data.SetData(receivedData);
        return data;
    }

    /// <summary>
    /// This method is used to load all data from PlayFab. But before that, it marks all data as dirty.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public async UniTask<IData> LoadAll(IData data)
    {
        // Cache local data history
        var localDataHistory = data.GetData<GameDataHistory>(GameDataType.GameDataHistory);

        // Mark all data as dirty to request all data
        data.MarkAllDirty();

        // Send request to PlayFab for getting all data
        var receivedData = await GetFromPlayFab(data.GetDirtyDataTypes());

        // Check if received data is null or empty
        if (receivedData is null || receivedData.Count == 0)
            return null;

        // Check if received data contains GameDataHistory
        if (!receivedData.TryGetValue(nameof(GameDataType.GameDataHistory), out var jsonObject))
            return null;

        // Deserialize GameDataHistory from received data
        var newGameDataHistory = JsonConvert.DeserializeObject<GameDataHistory>(jsonObject).Records;
        var newGameDataHistoryList = newGameDataHistory.ToList();

        // Sort by value in descending order
        newGameDataHistoryList.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));

        // Get biggest revision number tupple
        var latestDataUpdateRecord = newGameDataHistoryList.First();

        // Check if the latest data is updated by this device 
        if (latestDataUpdateRecord.Key.Equals(SystemInfo.deviceUniqueIdentifier))
        {
            switch (localDataHistory.Records[SystemInfo.deviceUniqueIdentifier].CompareTo(latestDataUpdateRecord.Value))
            {
                case -1:
                    // Son update eden bu cihaz ama data versiyonu remote database'de daha buyuk, local'de bir kayıp var demektir.
                    data.SetData(receivedData);
                    break;
                case 0:
                    return null;
                case 1:
                    data.MarkAllDirty();
                    await SendToPlayFab(data, data.GetDirtyDataTypes());
                    break;
            }
        }
        else
        {
            // Check if this device has any records in the GameDataHistory of remote server side
            if (!newGameDataHistory.ContainsKey(SystemInfo.deviceUniqueIdentifier))
            {
                // TODO: Burada soru soracağız remote'ta farklı bizde farklı data var hangisini kullanmak istiyorsun?
                await AskDataSyncSelectionToUser(data, receivedData, localDataHistory, latestDataUpdateRecord);
            }
            // That means this device has a record in the GameDataHistory of remote server side
            else
            {
                switch (newGameDataHistory[SystemInfo.deviceUniqueIdentifier]
                            .CompareTo(localDataHistory.Records[SystemInfo.deviceUniqueIdentifier]))
                {
                    case -1: // That means remote data is older than local data
                        await AskDataSyncSelectionToUser(data, receivedData, localDataHistory, latestDataUpdateRecord);
                        break;
                    case 0: // That means remote data is same as local data
                    case 1: // That means remote data is newer than local data
                        data.SetData(receivedData);
                        break;
                }
            }
        }

        return data;
    }

    private async UniTask AskDataSyncSelectionToUser(IData data, Dictionary<string, string> receivedData,
        GameDataHistory localDataHistory,
        KeyValuePair<string, uint> latestDataUpdateRecord)
    {
        #region Create ProgressSummaryCardModel for Local data

        var localGameStateData = data.GetData(GameDataType.State) as Dictionary<string, ulong>;
        if (!localGameStateData.TryGetValue(PersistentKeys.State.MaxLevelIndex, out ulong localLevel))
            localLevel = 0;

        var localExchangeData = data.GetData(GameDataType.ExchangeData) as Dictionary<string, TypeStringTuple>;
        var localCoin =
            (int)GameInstaller.Instance.SystemLocator.ExchangeManager.GetExchange(localExchangeData, CurrencyExtension.GetString(Currency.Coin),
                0f);

        BasicProgressSummaryCardModel localBasicProgressSummaryCardData = new BasicProgressSummaryCardModel()
        {
            dataSourceType = DataSourceType.Local,
            title = nameof(DataSourceType.Local),
            level = Convert.ToInt32(localLevel),
            coins = localCoin,
        };

        #endregion

        #region Create ProgressSummaryCardModel for Remote data

        var remoteGameStateData =
            JsonConvert.DeserializeObject<Dictionary<string, ulong>>(receivedData[GameDataType.State.ToString()]);
        var remoteLevel = (int)remoteGameStateData[PersistentKeys.State.MaxLevelIndex];
        var remoteExchangeData =
            JsonConvert.DeserializeObject<Dictionary<string, TypeStringTuple>>(
                receivedData[GameDataType.ExchangeData.ToString()]);
        var remoteCoin =
            (int)GameInstaller.Instance.SystemLocator.ExchangeManager.GetExchange(remoteExchangeData, CurrencyExtension.GetString(Currency.Coin),
                0f);

        BasicProgressSummaryCardModel remoteBasicProgressSummaryCardData = new BasicProgressSummaryCardModel()
        {
            dataSourceType = DataSourceType.Remote,
            title = nameof(DataSourceType.Remote),
            level = remoteLevel,
            coins = remoteCoin,
        };

        #endregion

        // Create DataSyncSelectPopupData
        int selectedDataSourceType = -1;

        GameInstaller.Instance.SystemLocator.EventManager.Trigger(new Events.AskDataSyncSourceEvent(
            remoteBasicProgressSummaryCardData, localBasicProgressSummaryCardData,
            (dataSourceType) => selectedDataSourceType = (int)dataSourceType));
        
        // Wait until user selects a data source type
        await UniTask.WaitUntil(() => selectedDataSourceType != -1);

        // Check if user selected remote data then overwrite it on local data
        if ((DataSourceType)selectedDataSourceType is DataSourceType.Remote)
        {
            data.SetData(receivedData);
        }
        // If not then mark all local data as dirty to send it to remote server
        else
        {
            data.MarkAllDirty();
        }

        // Update GameDataHistory
        localDataHistory.Records[SystemInfo.deviceUniqueIdentifier] = latestDataUpdateRecord.Value + 1;

        // Updagte GameDataHistory with latest version number of remote GameDataHistory
        Dictionary<string, string> newGameDataHistoryDict = new Dictionary<string, string>();
        newGameDataHistoryDict.Add(GameData.GameDataBiMap.KeyMap[GameDataType.GameDataHistory],
            JsonHelper.ToJson(localDataHistory));

        data.SetData(newGameDataHistoryDict, true);

        Save(data);
    }


    /// <summary>
    /// This method is used to save data to PlayFab according to the dirtyDataTypes.
    /// </summary>
    /// <param name="data"></param>
    public async void Save(IData data)
    {
        if (!PlayFabClientAPI.IsClientLoggedIn())
            return;

        await SendToPlayFab(data, data.GetDirtyDataTypes());
    }

    public async UniTask SaveRemote(IData data)
    {
        if (!PlayFabClientAPI.IsClientLoggedIn())
            return;

        await SendToPlayFab(data, data.GetDirtyDataTypes());
    }

    /// <summary>
    /// This method is used to save all data to PlayFab. But before that, it marks all data as dirty.
    /// </summary>
    /// <param name="data"></param>
    public async void SaveAll(IData data)
    {
        data.MarkAllDirty();

        await SendToPlayFab(data, data.GetDirtyDataTypes());
    }

    public async UniTask SendToPlayFab(IData data, HashSet<GameDataType> dirtyList,
        UserDataPermission permission = UserDataPermission.Private)
    {
        Dictionary<string, string> _nonsharedRawBatchData = new Dictionary<string, string>();
        Dictionary<string, string> _sharedRawBatchData = new Dictionary<string, string>();
        List<Dictionary<string, string>> _nonsharedDataBatches = new List<Dictionary<string, string>>();
        List<Dictionary<string, string>> _sharedDataBatches = new List<Dictionary<string, string>>();

        // Get all dirty data instances and add them to batch data dict
        foreach (var dirtyDataType in dirtyList)
        {
            var jsonObject = JsonHelper.ToJson(data.GetData(dirtyDataType));
            if (GameData.GameDataBiMap.KeyMap.TryGetValue(dirtyDataType, out string nonsharedDataTypeKey))
            {
                _nonsharedRawBatchData.Add(nonsharedDataTypeKey, jsonObject);
            }
            else if (GameData.GameDataBiMapShared.KeyMap.TryGetValue(dirtyDataType, out string sharedDataTypeKey))
            {
                _sharedRawBatchData.Add(sharedDataTypeKey, jsonObject);
            }
        }

        _nonsharedDataBatches.AddRange(_dataBatcher.GetBatchedData(_nonsharedRawBatchData, data));
        _sharedDataBatches.AddRange(_dataBatcher.GetBatchedData(_sharedRawBatchData, data));

        UpdateUserDataRequest request;
        bool responseReceived;

        foreach (var dataBatch in _nonsharedDataBatches)
        {
            responseReceived = false;

            request = new UpdateUserDataRequest()
            {
                Data = dataBatch,
                Permission = UserDataPermission.Private,
            };

            PlayFabClientAPI.UpdateUserData(request, OnUpdateUserDataResponse, OnErrorResponse);

            await UniTask.WaitUntil(() => responseReceived);
        }

        foreach (var dataBatch in _sharedDataBatches)
        {
            responseReceived = false;

            request = new UpdateUserDataRequest()
            {
                Data = dataBatch,
                Permission = UserDataPermission.Public,
            };

            PlayFabClientAPI.UpdateUserData(request, OnUpdateUserDataResponse, OnErrorResponse);

            await UniTask.WaitUntil(() => responseReceived);
        }

        void OnErrorResponse(PlayFabError error)
        {
            Debug.LogError(error.GenerateErrorReport());
            responseReceived = true;
        }

        void OnUpdateUserDataResponse(UpdateUserDataResult response)
        {
            Debug.Log(response.ToJson());
            responseReceived = true;
        }
    }

    /// <summary>
    /// This method is used to get data from PlayFab according to the requestedDataList.
    /// </summary>
    /// <param name="requestedDataList"></param>
    /// <returns></returns>
    public async UniTask<Dictionary<string, string>> GetFromPlayFab(HashSet<GameDataType> requestedDataList)
    {
        var keylist = requestedDataList.Select(gameDataType => GameData.GetGameDataNameOf(gameDataType)).ToList();
        Dictionary<string, string> _nonsharedRawBatchData = new Dictionary<string, string>();

        // Get dirty data types
        GetUserDataRequest request = new GetUserDataRequest()
        {
            Keys = keylist,
        };

        PlayFabClientAPI.GetUserData(request, OnGetUserDataResponse, OnErrorResponse);

        bool responseReceived = false;

        void OnGetUserDataResponse(GetUserDataResult response)
        {
            Debug.Log(response.Data);

            _nonsharedRawBatchData.Clear();

            foreach (var dataRecord in response.Data)
            {
                _nonsharedRawBatchData.Add(dataRecord.Key, dataRecord.Value.Value);
            }

            responseReceived = true;
        }

        void OnErrorResponse(PlayFabError error)
        {
            Debug.LogError(error.GenerateErrorReport());
            responseReceived = true;
        }

        await UniTask.WaitUntil(() => responseReceived);
        return _nonsharedRawBatchData;
    }
}