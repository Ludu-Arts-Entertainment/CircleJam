public partial class SystemLocator
{
    private UIManager _uiManager;
    public UIManager UIManager =>
        _uiManager ??= GameInstaller.Instance.ManagerDictionary[ManagerEnums.UIManager] as UIManager;
}
