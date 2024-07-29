public partial class SystemLocator
{
    private WatchToEarnManager _watchToEarnManager;
    public WatchToEarnManager WatchToEarnManager =>
        _watchToEarnManager ??= GameInstaller.Instance.ManagerDictionary[ManagerEnums.WatchToEarnManager] as WatchToEarnManager;
}
