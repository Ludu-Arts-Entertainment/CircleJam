using System;

[System.Serializable]
public class AdConfig : IConfig
{
    public bool InterstitialAdEnabled = true;
    public bool RewardedAdEnabled = true;
    public bool BannerAdEnabled = true;
    public int InterstitialInterval = 30;
    public int RewardedAdCooldown = 30;
    public int FirstInterstitialAdDelay = 30;
    public int FirstInterstitialLevel = 1;
    public int GameOpenInterstitialAdDelay = 30;
}