public partial class SystemLocator
{
    private GridManager _gridManager;
    public GridManager GridManager =>
        _gridManager ??= GameInstaller.Instance.ManagerDictionary[ManagerEnums.GridManager] as GridManager;
}