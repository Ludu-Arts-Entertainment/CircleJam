#if !DataManager_Modified

using System;
using System.Collections.Generic;
using DataStructures.BiDictionary;
using Newtonsoft.Json;

[Serializable]
public partial class GameData : IData
{
    [JsonIgnore] public static readonly BiDictionary<GameDataType, string> GameDataBiMap = new()
    {
        {GameDataType.State , nameof(GameDataType.State)},
        {GameDataType.SettingsFloat , nameof(GameDataType.SettingsFloat)},
        {GameDataType.SettingsString , nameof(GameDataType.SettingsString)},
        {GameDataType.StoreTransactionHistory , nameof(GameDataType.StoreTransactionHistory)},
        {GameDataType.ExchangeData , nameof(GameDataType.ExchangeData)},
        {GameDataType.InventoryData , nameof(GameDataType.InventoryData)},
        {GameDataType.QuestData , nameof(GameDataType.QuestData)},
        {GameDataType.SpecialOfferData , nameof(GameDataType.SpecialOfferData)},
        {GameDataType.ProfileData , nameof(GameDataType.ProfileData)},
        {GameDataType.PlayerStatData , nameof(GameDataType.PlayerStatData)},
        {GameDataType.TutorialData , nameof(GameDataType.TutorialData)},
        {GameDataType.LastRouletteUpdateTime , nameof(GameDataType.LastRouletteUpdateTime)},
        {GameDataType.RouletteData , nameof(GameDataType.RouletteData)},
        {GameDataType.DailyOfferData , nameof(GameDataType.DailyOfferData)},
        {GameDataType.ProfileSummaryData , nameof(GameDataType.ProfileSummaryData)},
        {GameDataType.GameDataHistory , nameof(GameDataType.GameDataHistory)},
        {GameDataType.LoginStatusData , nameof(GameDataType.LoginStatusData)}
    };
    [JsonIgnore]
    public static readonly BiDictionary<GameDataType, string> GameDataBiMapShared = new()
    {
        {GameDataType.ProfileSummaryData , nameof(GameDataType.ProfileSummaryData)},
    };
    [JsonIgnore] private HashSet<GameDataType> _dirtyDataTypes = new HashSet<GameDataType>();
    
    public Dictionary<string,ulong> State = new Dictionary<string, ulong>();
    public GameDataHistory GameDataHistory = new GameDataHistory();
    public ProfileSummaryData ProfileSummaryData = new ProfileSummaryData();
    
    public static string GetGameDataNameOf(GameDataType dataType)
    {
        if (GameDataBiMap.KeyMap.TryGetValue(dataType, out var gameDataTypeName))
        {
            return gameDataTypeName;
        }
        
        if (GameDataBiMapShared.KeyMap.TryGetValue(dataType, out var gameDataTypeNameShared))
        {
            return gameDataTypeNameShared;
        }
        
        throw new Exception($"GameDataBiMap and GameDataBiMapShared does not contain {dataType}");
    }
    public static GameDataType GetGameDataTypeOf(string dataTypeName)
    {
        if (GameDataBiMap.ValueMap.TryGetValue(dataTypeName, out var gameDataType))
        {
            return gameDataType;
        }
        
        if (GameDataBiMapShared.ValueMap.TryGetValue(dataTypeName, out var gameDataTypeShared))
        {
            return gameDataTypeShared;
        }
        
        throw new Exception($"GameDataBiMap and GameDataBiMapShared does not contain {dataTypeName}");
    }
    /// <summary>
    /// This method is used to mark a data type as dirty.
    /// </summary>
    /// <param name="key"></param>
    private void MarkDirty(GameDataType key)
    {
        _dirtyDataTypes.Add(key);
    }
    
    /// <summary>
    /// This method is used to clear dirty data types list.
    /// </summary>
    public void ClearDirty()
    {
        _dirtyDataTypes.Clear();
    }
    
    /// <summary>
    /// This method is used to check if a data type is dirty or not.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool IsDirty(GameDataType key)
    {
        return _dirtyDataTypes.Contains(key);
    }
    
    /// <summary>
    /// Mark all data types as dirty.
    /// </summary>
    public void MarkAllDirty()
    {
        foreach (GameDataType gameDataType in Enum.GetValues(typeof(GameDataType)))
        {
            _dirtyDataTypes.Add(gameDataType);
        }
    }
    
    /// <summary>
    /// This method is used to get dirty data types hashset.
    /// </summary>
    /// <returns></returns>
    public HashSet<GameDataType> GetDirtyDataTypes()
    {
        return _dirtyDataTypes;
    }
    public T GetData<T>(GameDataType key) 
    {
        switch (key)
        {
            case GameDataType.State:
                return (T)Convert.ChangeType(State, typeof(T));
#if SettingManager_Enabled
            case GameDataType.SettingsFloat:
                return (T)Convert.ChangeType(SettingFloat, typeof(T));
            case GameDataType.SettingsString:
                return (T)Convert.ChangeType(SettingString, typeof(T));
#endif
#if ProductManager_Enabled
            case GameDataType.StoreTransactionHistory:
            return (T)Convert.ChangeType(StoreTransactionHistory, typeof(T));
#endif
#if ExchangeManager_Enabled
            case GameDataType.ExchangeData:
                return (T)Convert.ChangeType(ExchangeData, typeof(T));
#endif
#if InventoryManager_Enabled
            case GameDataType.InventoryData:
                return (T)Convert.ChangeType(InventoryItems, typeof(T));
#endif
#if QuestManager_Enabled
            case GameDataType.QuestData:
                return (T)Convert.ChangeType(QuestData, typeof(T));
#endif
#if SpecialOfferManager_Enabled
            case GameDataType.SpecialOfferData:
                return (T)Convert.ChangeType(ActiveSpecialOfferId, typeof(T));
#endif
#if TutorialManager_Enabled
            case GameDataType.TutorialData:
                return (T)Convert.ChangeType(TutorialData, typeof(T));
#endif
#if DailyOfferManager_Enabled
            case GameDataType.DailyOfferData:
                return (T)Convert.ChangeType(DailyOfferData, typeof(T));
            // case GameDataType.LastDailyOfferUpdateTime:
            //     return (T)Convert.ChangeType(LastDailyOfferUpdateTime, typeof(T));
#endif
#if RouletteManager_Enabled
            case GameDataType.RouletteData:
                return (T)Convert.ChangeType(RouletteData, typeof(T));
            case GameDataType.LastRouletteUpdateTime:
                return (T)Convert.ChangeType(LastRouletteUpdateTime, typeof(T));
#endif
            case GameDataType.PlayerStatData:
                return (T)Convert.ChangeType(PlayerStatData, typeof(T));
            case GameDataType.ProfileData:
                return (T)Convert.ChangeType(ProfileData, typeof(T));
            case GameDataType.ProfileSummaryData:
                return (T)Convert.ChangeType(ProfileSummaryData, typeof(T));
            case GameDataType.GameDataHistory:
                return (T)Convert.ChangeType(GameDataHistory, typeof(T));
#if LoginManager_Enabled
            case GameDataType.LoginStatusData:
                return (T)Convert.ChangeType(LoginStatusData, typeof(T));
#endif 
            default:
                return default;
        }
    }
    public object GetData(GameDataType key)
    {
        switch (key)
        {
            case GameDataType.State:
                return State;
#if SettingManager_Enabled
            case GameDataType.SettingsFloat:
                return SettingFloat;
            case GameDataType.SettingsString:
                return SettingString;
#endif
#if ProductManager_Enabled
            case GameDataType.StoreTransactionHistory:
                return StoreTransactionHistory;
#endif
#if ExchangeManager_Enabled
            case GameDataType.ExchangeData:
                return ExchangeData;
#endif
#if InventoryManager_Enabled
            case GameDataType.InventoryData:
                return InventoryItems;
#endif
#if QuestManager_Enabled
            case GameDataType.QuestData:
                return QuestData;
#endif
#if SpecialOfferManager_Enabled
            case GameDataType.SpecialOfferData:
                return ActiveSpecialOfferId;
#endif
#if TutorialManager_Enabled
            case GameDataType.TutorialData:
                return TutorialData;
#endif
#if DailyOfferManager_Enabled
            case GameDataType.DailyOfferData:
                return DailyOfferData;
#endif
#if RouletteManager_Enabled
            case GameDataType.RouletteData:
                return RouletteData;
            case GameDataType.LastRouletteUpdateTime:
                return LastRouletteUpdateTime;
#endif
            case GameDataType.PlayerStatData:
                return PlayerStatData;
            case GameDataType.ProfileData:
                return ProfileData;
            case GameDataType.ProfileSummaryData:
                return ProfileSummaryData;
            case GameDataType.GameDataHistory:
                return GameDataHistory;
#if LoginManager_Enabled
            case GameDataType.LoginStatusData:
                return LoginStatusData;
#endif
            default:
                return default;
        }
    }
    public void SetData<T>(GameDataType key, T value)
    {
        switch (key)
        {
            case GameDataType.State:
                State = (Dictionary<string, ulong>)Convert.ChangeType(value, typeof(Dictionary<string, ulong>));
                break;
#if SettingManager_Enabled
            case GameDataType.SettingsFloat:
                SettingFloat = (Dictionary<SettingType, float>)Convert.ChangeType(value, typeof(Dictionary<SettingType, float>));
                break;
            case GameDataType.SettingsString:
                SettingString = (Dictionary<SettingType, string>)Convert.ChangeType(value, typeof(Dictionary<SettingType, string>));
                break;
#endif
#if ProductManager_Enabled
            case GameDataType.StoreTransactionHistory:
                StoreTransactionHistory = (List<Dictionary<string,List<StoreTransaction>>>)Convert.ChangeType(value, typeof(List<Dictionary<string,List<StoreTransaction>>>));
                break;
#endif
#if ExchangeManager_Enabled
            case GameDataType.ExchangeData:
                ExchangeData = (Dictionary<string, TypeStringTuple>)Convert.ChangeType(value, typeof(Dictionary<string, TypeStringTuple>));
                break;
#endif
#if InventoryManager_Enabled
            case GameDataType.InventoryData:
                InventoryItems = (Dictionary<string,Dictionary<string,Dictionary<Type,List<string>>>>)Convert.ChangeType(value, typeof(Dictionary<string,Dictionary<string,Dictionary<Type,List<string>>>>));
                break;
#endif
#if QuestManager_Enabled
            case GameDataType.QuestData:
                QuestData = (Dictionary<QuestGroupStatus,Dictionary<string,QuestGroupInfo>>)Convert.ChangeType(value, typeof(Dictionary<QuestGroupStatus,Dictionary<string,QuestGroupInfo>>));
                break;
#endif
#if SpecialOfferManager_Enabled
            case GameDataType.SpecialOfferData:
                ActiveSpecialOfferId = (string)Convert.ChangeType(value, typeof(string));
                break;
#endif
#if TutorialManager_Enabled
            case GameDataType.TutorialData:
                TutorialData = (Dictionary<TutorialType, TutorialState>)Convert.ChangeType(value, typeof(Dictionary<TutorialType, TutorialState>));
                break;
#endif
#if DailyOfferManager_Enabled
            case GameDataType.DailyOfferData:
                DailyOfferData = (Dictionary<int, DailyOfferSaveData>)Convert.ChangeType(value,
                    typeof(Dictionary<int, DailyOfferSaveData>));
                break;
#endif
#if RouletteManager_Enabled
            case GameDataType.RouletteData:
                RouletteData = (Dictionary<int, RouletteSaveData>)Convert.ChangeType(value,
                    typeof(Dictionary<int, RouletteSaveData>));
                break;
            case GameDataType.LastRouletteUpdateTime:
                LastRouletteUpdateTime = (DateTime)Convert.ChangeType(value, typeof(DateTime));
                break;
#endif
            case GameDataType.PlayerStatData:
                PlayerStatData = (Dictionary<string, int>)Convert.ChangeType(value, typeof(Dictionary<string, int>));
                break;
            case GameDataType.ProfileSummaryData:
                ProfileSummaryData = (ProfileSummaryData)Convert.ChangeType(value, typeof(ProfileSummaryData));
                break;
            case GameDataType.ProfileData:
                ProfileData = (ProfileModel)Convert.ChangeType(value, typeof(ProfileModel));
                break;
            case GameDataType.GameDataHistory:
                GameDataHistory = (GameDataHistory)Convert.ChangeType(value, typeof(GameDataHistory));
                break;
#if LoginManager_Enabled
            case GameDataType.LoginStatusData:
                LoginStatusData = (LoginStatusModel)Convert.ChangeType(value, typeof(LoginStatusModel));
                break;
#endif 
                
            default:
                return;
        }
        MarkDirty(key);
    }

    public void SetData(Dictionary<string, string> data, bool markDirty = false)
    {
        foreach (var gameDataTypeName in data.Keys)
        {
            switch (gameDataTypeName)
            {
                case nameof(GameDataType.State):
                    State = JsonConvert.DeserializeObject<Dictionary<string, ulong>>(data[gameDataTypeName]);
                    break;
#if SettingManager_Enabled
                case nameof(GameDataType.SettingsFloat):
                    SettingFloat = JsonConvert.DeserializeObject<Dictionary<SettingType, float>>(data[gameDataTypeName]);
                    break;
                case nameof(GameDataType.SettingsString):
                    SettingString = JsonConvert.DeserializeObject<Dictionary<SettingType, string>>(data[gameDataTypeName]);
                    break;
#endif
#if ProductManager_Enabled
                case nameof(GameDataType.StoreTransactionHistory):
                    StoreTransactionHistory = JsonConvert.DeserializeObject<List<Dictionary<string,List<StoreTransaction>>>>(data[gameDataTypeName]);
                    break;
#endif
#if ExchangeManager_Enabled
                case nameof(GameDataType.ExchangeData):
                    ExchangeData = JsonConvert.DeserializeObject<Dictionary<string, TypeStringTuple>>(data[gameDataTypeName]);
                    break;
#endif
#if InventoryManager_Enabled
                case nameof(GameDataType.InventoryData):
                    InventoryItems = JsonConvert.DeserializeObject<Dictionary<string,Dictionary<string,Dictionary<Type,List<string>>>>>(data[gameDataTypeName]);
                    break;
#endif
#if QuestManager_Enabled
                case nameof(GameDataType.QuestData):
                    QuestData = JsonConvert.DeserializeObject<Dictionary<QuestGroupStatus,Dictionary<string,QuestGroupInfo>>>(data[gameDataTypeName]);
                    break;
#endif
#if SpecialOfferManager_Enabled
                case nameof(GameDataType.SpecialOfferData):
                    ActiveSpecialOfferId = JsonConvert.DeserializeObject<string>(data[gameDataTypeName]);
                    break;
#endif
#if TutorialManager_Enabled
                case nameof(GameDataType.TutorialData):
                    TutorialData = JsonConvert.DeserializeObject<Dictionary<TutorialType, TutorialState>>(data[gameDataTypeName]);
                    break;
#endif
#if DailyOfferManager_Enabled
                case nameof(GameDataType.DailyOfferData):
                    DailyOfferData = JsonConvert.DeserializeObject<Dictionary<int, DailyOfferSaveData>>(data[gameDataTypeName]);
                    break;
#endif
#if RouletteManager_Enabled
                case nameof(GameDataType.RouletteData):
                    RouletteData = JsonConvert.DeserializeObject<Dictionary<int, RouletteSaveData>>(data[gameDataTypeName]);
                    break;
                case nameof(GameDataType.LastRouletteUpdateTime):
                    LastRouletteUpdateTime = JsonConvert.DeserializeObject<DateTime>(data[gameDataTypeName]);
                    break;
#endif
                case nameof(GameDataType.PlayerStatData):
                    PlayerStatData = JsonConvert.DeserializeObject<Dictionary<string, int>>(data[gameDataTypeName]);
                    break;
                case nameof(GameDataType.ProfileSummaryData):
                    ProfileSummaryData = JsonConvert.DeserializeObject<ProfileSummaryData>(data[gameDataTypeName]);
                    break;
                case nameof(GameDataType.ProfileData):
                    ProfileData = JsonConvert.DeserializeObject<ProfileModel>(data[gameDataTypeName]);
                    break;
                case nameof(GameDataType.GameDataHistory):
                    GameDataHistory = JsonConvert.DeserializeObject<GameDataHistory>(data[gameDataTypeName]);
                    break;
#if LoginManager_Enabled
                case nameof(GameDataType.LoginStatusData):
                    LoginStatusData = JsonConvert.DeserializeObject<LoginStatusModel>(data[gameDataTypeName]);
                    break;
#endif
                default:
                    return;
            }
            if (markDirty)
            {
                MarkDirty(GameDataBiMap.ValueMap[gameDataTypeName]);
            }
        }
    }

    public Type GetDataType()
    {
        return typeof(GameData);
    }
}

public enum GameDataType
{
    State,
    SettingsFloat,
    SettingsString,
    StoreTransactionHistory,
    ExchangeData,
    InventoryData,
    QuestData,
    SpecialOfferData,
    ProfileData,
    PlayerStatData,
    TutorialData,
    LastRouletteUpdateTime,
    RouletteData,
    DailyOfferData,
    ProfileSummaryData,
    GameDataHistory,
    LoginStatusData
}
#endif