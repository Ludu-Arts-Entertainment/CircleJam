#if !PersestentKeys_Modified
public static partial class PersistentKeys
{
    public static class Authentication
    {
        public const string PlayFabId = "PlayFabId";
        public const string IsLoggedInWithFacebook = "IsLoggedInWithFacebook";
        public const string FacebookAccessToken = "FacebookAccessToken";
    }
    public static string IsFirstLogin => "IsFirstLogin";
    public const string Environment = "Environment";
    public const string Language = "Language";
    public const string Music = "MusicLevel";
    public const string AudioLevel = "MusicLevel";
    public const string VibrationStatus = "VibrationStatus";
    public const string NotificationStatus = "NotificationStatus";
    
    #region Profile Datas

    public const string UserId = "UserId";
    public const string ProfileName = "ProfileName";
    public const string AvatarIndex = "AvatarIndex";
    public const string CountryCode = "CountryCode";
    
    #endregion
    
    public static class State
    {
        public const string MaxLevelIndex = "MaxLevelIndex";
    }
    
}
#endif