public partial class SystemLocator
{
    private InputManager _inputManager;
    public InputManager InputManager =>
        _inputManager ??= GameInstaller.Instance.ManagerDictionary[ManagerEnums.InputManager] as InputManager;
}