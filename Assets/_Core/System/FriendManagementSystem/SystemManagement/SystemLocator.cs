public partial class SystemLocator
{
    private FriendManager _friendManager;
    public FriendManager FriendManager => _friendManager ??= GameInstaller.Instance.ManagerDictionary[ManagerEnums.FriendManager] as FriendManager;
}
