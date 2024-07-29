using System;
using System.Collections.Generic;
using UnityEngine;
public class TestAdProvider : IAdProvider
{
    private AdConfig _adConfig;
    private float _lastInterstitialTime;

    public IAdProvider CreateSelf()
    {
        return new TestAdProvider();
    }

    public void Initialize(Action onReady)
    {
#if AdManager_Enabled
        _adConfig = GameInstaller.Instance.SystemLocator.RemoteConfigManager.GetObject<AdConfig>();
#else
        _adConfig = new AdConfig();
#endif
        onReady?.Invoke();
        if (GameInstaller.Instance.SystemLocator.DataManager.GetData<Dictionary<string, ulong>>(GameDataType.State)
                ["CurrentLevel"] == 1)
        {
            _lastInterstitialTime = Time.time + _adConfig.FirstInterstitialAdDelay;
        }
        else
            _lastInterstitialTime = Time.time + _adConfig.GameOpenInterstitialAdDelay;
//        Debug.Log($"[{this}] Initialized");

    }

    public void ShowInterstitial(Action onClosed = null)
    {
        if (!_adConfig.InterstitialAdEnabled)
        {
            onClosed?.Invoke();
            _lastInterstitialTime = Time.time + _adConfig.InterstitialInterval;
            return;
        }

        if (!CanShowInterstitial())
        {
            Debug.Log($"[{this}] Interstitial ad is not ready");
            return;
        }
        Debug.Log($"[{this}] ShowInterstitial");
        _lastInterstitialTime = Time.time + _adConfig.InterstitialInterval;
        onClosed?.Invoke();
    }

    public void ShowRewarded(Action onCompleted, Action onFailed = null)
    {
        if (!_adConfig.InterstitialAdEnabled)
        {
            onCompleted?.Invoke();
            _lastInterstitialTime = Time.time + _adConfig.RewardedAdCooldown;
            return;
        }
        onCompleted?.Invoke();
        _lastInterstitialTime = Time.time + _adConfig.RewardedAdCooldown;
        Debug.Log($"[{this}] ShowRewarded");

    }

    public void ShowBanner()
    {
        Debug.Log($"[{this}] ShowBanner");
    }

    public void HideBanner()
    {
        Debug.Log($"[{this}] HideBanner");

    }

    public bool CanShowInterstitial()
    {
        return _lastInterstitialTime < Time.time && (ulong)_adConfig.FirstInterstitialLevel <=
            GameInstaller.Instance.SystemLocator.DataManager.GetData<Dictionary<string, ulong>>(GameDataType.State)
                ["CurrentLevel"];
    }
}
