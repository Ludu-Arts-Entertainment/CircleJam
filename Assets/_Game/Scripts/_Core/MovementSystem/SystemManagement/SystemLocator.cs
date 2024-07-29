public partial class SystemLocator
{
    private MovementManager _movementManager;
    public MovementManager MovementManager =>
        _movementManager ??= GameInstaller.Instance.ManagerDictionary[ManagerEnums.MovementManager] as MovementManager;
}