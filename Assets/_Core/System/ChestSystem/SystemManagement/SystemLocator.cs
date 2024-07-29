public partial class SystemLocator
{
    private ChestManager _chestManager;
    public ChestManager ChestManager => _chestManager ??= GameInstaller.Instance.ManagerDictionary[ManagerEnums.ChestManager] as ChestManager;
}
