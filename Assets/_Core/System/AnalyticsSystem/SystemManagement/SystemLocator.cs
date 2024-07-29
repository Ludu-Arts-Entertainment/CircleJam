public partial class SystemLocator
{
    private AnalyticsManager _analyticsManager;
    public AnalyticsManager AnalyticsManager=> _analyticsManager ??= GameInstaller.Instance.ManagerDictionary[ManagerEnums.AnalyticsManager] as AnalyticsManager;
}
