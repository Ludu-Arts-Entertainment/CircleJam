public partial class SystemLocator
{
    private LeaderboardManager _leaderboardManager;
    public LeaderboardManager LeaderboardManager=> _leaderboardManager ?? GameInstaller.Instance.ManagerDictionary[ManagerEnums.LeaderboardManager] as LeaderboardManager;

}