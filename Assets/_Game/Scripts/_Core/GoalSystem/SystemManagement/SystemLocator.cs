public partial class SystemLocator
{
    private GoalManager _goalManager;
    public GoalManager GoalManager =>
        _goalManager ??= GameInstaller.Instance.ManagerDictionary[ManagerEnums.GoalManager] as GoalManager;
}