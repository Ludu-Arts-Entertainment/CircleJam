using System.Collections.Generic;

public enum LeaderboardProviderEnums
{
    FakeLeaderboardProvider,
#if PlayFabSdk_Enabled
    PlayFabLeaderboardProvider,
#endif
}
public static class LeaderboardProviderFactory
{
    private static readonly Dictionary<LeaderboardProviderEnums,ILeaderboardProvider> LeaderboardProviderDictionary = new ()
    {
        {LeaderboardProviderEnums.FakeLeaderboardProvider, new FakeLeaderboardProvider()},
#if PlayFabSdk_Enabled
        {LeaderboardProviderEnums.PlayFabLeaderboardProvider, new PlayfabLeaderboardProvider()},
#endif
    };
    
    public static ILeaderboardProvider Create(LeaderboardProviderEnums providerEnum)
    {
        return LeaderboardProviderDictionary.TryGetValue(providerEnum, out var provider) ? provider.CreateSelf() : null;
    }
}
