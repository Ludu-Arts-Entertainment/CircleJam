public partial class SystemLocator
{
    private DataManager _dataManager;
    public DataManager DataManager =>
        _dataManager ??= GameInstaller.Instance.ManagerDictionary[ManagerEnums.DataManager] as DataManager;
}
