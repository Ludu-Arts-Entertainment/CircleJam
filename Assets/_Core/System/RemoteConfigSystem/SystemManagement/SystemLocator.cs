public partial class SystemLocator
{
    private RemoteConfigManager _remoteConfigManager;
    public RemoteConfigManager RemoteConfigManager =>
        _remoteConfigManager ??= GameInstaller.Instance.ManagerDictionary[ManagerEnums.RemoteConfigManager] as RemoteConfigManager;
}