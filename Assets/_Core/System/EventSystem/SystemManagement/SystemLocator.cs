public partial class SystemLocator
{
    private EventManager _eventManager;
    public EventManager EventManager =>
        _eventManager ??= GameInstaller.Instance.ManagerDictionary[ManagerEnums.EventManager] as EventManager;
}
