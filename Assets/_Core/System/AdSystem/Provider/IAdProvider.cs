using System;

public interface IAdProvider
{
    IAdProvider CreateSelf();
    void Initialize(Action onReady);
    void ShowInterstitial(Action onClosed = null);
    void ShowRewarded(Action onCompleted, Action onFailed = null);
    void ShowBanner();
    void HideBanner();
    bool CanShowInterstitial();
}
