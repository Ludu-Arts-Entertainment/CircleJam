using System.Collections.Generic;

public enum FriendProviderEnums
{
    DummyFriendProvider,
#if PlayFabSdk_Enabled
    PlayFabFriendProvider,
#endif
}
public static class FriendProviderFactory
{
    private static readonly Dictionary<FriendProviderEnums, IFriendProvider> FriendProviderDictionary = new()
    {
        { FriendProviderEnums.DummyFriendProvider, new DummyFriendProvider() },
#if PlayFabSdk_Enabled
        { FriendProviderEnums.PlayFabFriendProvider, new PlayFabFriendProvider() },
#endif
    };
    
    public static IFriendProvider Create(FriendProviderEnums friendProviderEnums)
    {
        return FriendProviderDictionary.TryGetValue(friendProviderEnums, out var provider) ? provider.CreateSelf() : null;
    }
}
