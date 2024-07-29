public partial class SystemLocator
{
    private AdManager _adManager;
    public AdManager AdManager =>
        _adManager ??= GameInstaller.Instance.ManagerDictionary[ManagerEnums.AdManager] as AdManager;
}