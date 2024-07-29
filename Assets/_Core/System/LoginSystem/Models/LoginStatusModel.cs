using System;

[Serializable]
public class LoginStatusModel
{
    public bool IsLoginRequirementsMet;
    public bool IsPlayFabLoggedIn;
    public bool IsFacebookLoggedIn;
    public bool IsAppleLoggedIn;
    public bool IsGoogleLoggedIn;
}

[Serializable]
public class AppleLoginStatusModel
{
    public string AppleIdentityToken;
}

[Serializable]
public class GoogleLoginStatusModel
{
    public string GoogleIdentityToken;
}

[Serializable]
public class FacebookLoginStatusModel
{
    public string FacebookIdentityToken;
}