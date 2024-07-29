public partial class SystemLocator
{
    private DailyLoginManager _dailyLoginManager;
    public DailyLoginManager DailyLoginManager => _dailyLoginManager ??= GameInstaller.Instance.ManagerDictionary[ManagerEnums.DailyLoginManager] as DailyLoginManager;
}
