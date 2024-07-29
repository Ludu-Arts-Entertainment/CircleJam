public partial class SystemLocator
{
    private SettingManager _settingManager;
    public SettingManager SettingManager =>
        _settingManager ??= GameInstaller.Instance.ManagerDictionary[ManagerEnums.SettingManager] as SettingManager;
}