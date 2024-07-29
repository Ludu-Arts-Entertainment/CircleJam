
public partial class SystemLocator
{
    private HapticManager _hapticManager;
    public HapticManager HapticManager => _hapticManager ??= GameInstaller.Instance.ManagerDictionary[ManagerEnums.HapticManager] as HapticManager;
}
