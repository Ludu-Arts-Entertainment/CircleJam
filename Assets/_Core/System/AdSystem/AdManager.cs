using System;
public class AdManager : IManager
{
    // if you want use MaxSdk, you should add a MaxSdk_Enabled in Symbols
    private IAdProvider _adProvider;
    public IManager CreateSelf()
    {
        return new AdManager();
    }

    public void Initialize(GameInstaller gameInstaller, Action onReady)
    {
        
        _adProvider = AdProviderFactory.Create(gameInstaller.Customizer.AdProvider);
        _adProvider.Initialize(onReady );
    }

    public bool IsReady()
    {
        return _adProvider != null;
    }
    public void ShowInterstitial(Action onClosed = null)
    {
        _adProvider.ShowInterstitial(onClosed);
    }
    public void ShowRewarded(Action onCompleted, Action onFailed = null)
    {
        _adProvider.ShowRewarded(onCompleted, onFailed);
    }
    public void ShowBanner()
    {
        _adProvider.ShowBanner();
    }
    public void HideBanner()
    {
        _adProvider.HideBanner();
    }
}