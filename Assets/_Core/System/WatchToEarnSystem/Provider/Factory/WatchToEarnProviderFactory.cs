using System.Collections.Generic;

public static class WatchToEarnProviderFactory
{
    private static readonly Dictionary<WatchToEarnProviderType, IWatchToEarnProvider> Providers = new()
    {
        {WatchToEarnProviderType.Basic, new BasicWatchToEarnProvider()},
    };
    public static IWatchToEarnProvider Create(WatchToEarnProviderType type)
    {
        return Providers.TryGetValue(type, out var provider) ? provider.CreateSelf() : null;
    }
}
public enum WatchToEarnProviderType
{
    Basic
}