using System.Collections.Generic;

public enum ManagerEnums
{
    DataManager,
    EventManager,
    PoolManager,
    LevelManager,
    UIManager,
    InputManager,
    SettingManager,
    AudioManager,
    HapticManager,
    AdManager,
    RemoteConfigManager,
    ProductManager,
    ExchangeManager,
    InventoryManager,
    QuestManager,
    ChestManager,
    EnergyManager,
    AnalyticsManager,
    LoginManager,
    LeaderboardManager,
    DailyLoginManager,
    SpecialOfferManager,
    WatchToEarnManager,
    TutorialManager,
    RouletteManager,
    DailyOfferManager,
    FriendManager,
    MailManager,
    GridManager,
    MovementManager,
}

public class ManagerFactory
{
    private static readonly Dictionary<ManagerEnums, IManager> ManagerDictionary =
        new Dictionary<ManagerEnums, IManager>()
        {
#if DataManager_Enabled
            { ManagerEnums.DataManager, new DataManager() },
#endif
#if EventManager_Enabled
            { ManagerEnums.EventManager, new EventManager() },
#endif
#if PoolManager_Enabled
            { ManagerEnums.PoolManager, new PoolManager() },
#endif
#if LevelManager_Enabled
            { ManagerEnums.LevelManager, new LevelManager() },
#endif
#if UIManager_Enabled
            { ManagerEnums.UIManager, new UIManager() },
#endif
#if InputManager_Enabled
            { ManagerEnums.InputManager, new InputManager() },
#endif
#if SettingManager_Enabled
            { ManagerEnums.SettingManager, new SettingManager() },
#endif
#if AudioManager_Enabled
            { ManagerEnums.AudioManager, new AudioManager() },
#endif
#if HapticManager_Enabled
            { ManagerEnums.HapticManager, new HapticManager() },
#endif
#if AdManager_Enabled
            { ManagerEnums.AdManager, new AdManager() },
#endif
#if RemoteConfigManager_Enabled
            { ManagerEnums.RemoteConfigManager, new RemoteConfigManager() },
#endif
#if ProductManager_Enabled
            { ManagerEnums.ProductManager, new ProductManager() },
#endif
#if ExchangeManager_Enabled
            { ManagerEnums.ExchangeManager, new ExchangeManager() },
#endif
#if InventoryManager_Enabled
            { ManagerEnums.InventoryManager, new InventoryManager() },
#endif
#if QuestManager_Enabled
            { ManagerEnums.QuestManager, new QuestManager() },
#endif
#if ChestManager_Enabled
            { ManagerEnums.ChestManager, new ChestManager() },
#endif
#if EnergyManager_Enabled
            { ManagerEnums.EnergyManager, new EnergyManager() },
#endif
#if AnalyticsManager_Enabled
            { ManagerEnums.AnalyticsManager, new AnalyticsManager() },
#endif
#if LoginManager_Enabled
            { ManagerEnums.LoginManager, new LoginManager() },
#endif
#if LeaderboardManager_Enabled
            { ManagerEnums.LeaderboardManager, new LeaderboardManager() },
#endif
#if DailyLoginManager_Enabled
            { ManagerEnums.DailyLoginManager, new DailyLoginManager() },
#endif
#if SpecialOfferManager_Enabled
            { ManagerEnums.SpecialOfferManager, new SpecialOfferManager() },
#endif
#if WatchToEarnManager_Enabled
            { ManagerEnums.WatchToEarnManager, new WatchToEarnManager() },
#endif
#if TutorialManager_Enabled
            { ManagerEnums.TutorialManager, new TutorialManager() },
#endif
#if RouletteManager_Enabled
            { ManagerEnums.RouletteManager, new RouletteManager() },
#endif
#if DailyLoginManager_Enabled
            { ManagerEnums.DailyOfferManager, new DailyOfferManager() },
#endif
#if FriendManager_Enabled
            { ManagerEnums.FriendManager, new FriendManager() },
#endif
#if MailManager_Enabled
            { ManagerEnums.MailManager, new MailManager() },
#endif
#if GridManager_Enabled
            { ManagerEnums.GridManager, new GridManager() },
#endif
#if MovementManager_Enabled
            { ManagerEnums.MovementManager, new MovementManager() },
#endif
        };

    public static IManager Create(ManagerEnums managerEnums)
    {
        var manager = ManagerDictionary[managerEnums].CreateSelf();
        return manager;
    }
}