public partial class SystemLocator
{
    private LoginManager _loginManager;
    public LoginManager LoginManager => _loginManager ??= GameInstaller.Instance.ManagerDictionary[ManagerEnums.LoginManager] as LoginManager;
}