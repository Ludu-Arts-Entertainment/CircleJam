public partial class SystemLocator
{
    private EnergyManager _energyManager;
    public EnergyManager EnergyManager => _energyManager ??= GameInstaller.Instance.ManagerDictionary[ManagerEnums.EnergyManager] as EnergyManager;
}