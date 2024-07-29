public partial class SystemLocator
{
    private PoolManager _poolManager;
    public PoolManager PoolManager =>
        _poolManager ??= GameInstaller.Instance.ManagerDictionary[ManagerEnums.PoolManager] as PoolManager;
}
