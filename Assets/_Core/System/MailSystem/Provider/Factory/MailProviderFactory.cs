using System.Collections.Generic;

public static class MailProviderFactory
{
    private static readonly Dictionary<MailProviderType, IMailProvider> MailProviders = new()
    {
        { MailProviderType.Dummy , new DummyMailProvider()},
#if PlayFabSdk_Enabled
        { MailProviderType.Playfab , new PlayfabMailProvider()},
#endif
    };
    
    public static IMailProvider Create(MailProviderType mailProviderType)
    {
        return MailProviders.GetValueOrDefault(mailProviderType)?.CreateSelf();
    }
}

public enum MailProviderType
{
    Dummy,
    Playfab
}