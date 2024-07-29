using System;
using System.Collections.Generic;

[Flags]
public enum LoginProviderEnums
{
    None = 0,
    DummyLoginProvider = 1,
    PlayFabLoginProvider = 2,
    FacebookLoginProvider = 4,
    GoogleLoginProvider = 8,
    AppleLoginProvider = 16,
}
    
public static class LoginProviderFactory
{
    private static Dictionary<LoginProviderEnums, ILoginProvider> _loginProviderDictionary = new ()
    {
        {LoginProviderEnums.DummyLoginProvider, new DummyLoginProvider()},
#if PlayFabSdk_Enabled 
        {LoginProviderEnums.PlayFabLoginProvider, new PlayFabLoginProvider()},
#endif
#if FacebookSdk_Enabled
        {LoginProviderEnums.FacebookLoginProvider, new FacebookLoginProvider()},
#endif
#if AppleAuth_Enabled
        {LoginProviderEnums.AppleLoginProvider, new AppleLoginProvider()},
#endif
    };

    public static ILoginProvider Create(LoginProviderEnums providerEnum)
    {
        return _loginProviderDictionary.TryGetValue(providerEnum, out var provider) ? provider.CreateSelf() : null;
    }
}