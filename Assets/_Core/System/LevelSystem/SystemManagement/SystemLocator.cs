public partial class SystemLocator
{
    private LevelManager _levelManager;
    public LevelManager LevelManager =>
        _levelManager ??= GameInstaller.Instance.ManagerDictionary[ManagerEnums.LevelManager] as LevelManager;
}
