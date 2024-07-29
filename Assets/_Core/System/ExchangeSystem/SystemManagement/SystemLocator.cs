public partial class SystemLocator
{
    private ExchangeManager _exchangeManager;
    public ExchangeManager ExchangeManager => _exchangeManager ??= GameInstaller.Instance.ManagerDictionary[ManagerEnums.ExchangeManager] as ExchangeManager;
}
