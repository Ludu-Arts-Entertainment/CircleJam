using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if MaxSdk_Enabled
public class ApplovinAdProvider : IAdProvider
{
#if UNITY_ANDROID || UNITY_EDITOR
    string adUnitIdInter = "null";
    string adUnitIdRewarded = "null";
    string adUnitIdBanner = "null";
#elif UNITY_IOS
        string adUnitIdInter = "null";
        string adUnitIdRewarded = "null";
        string adUnitIdBanner = "null";
#endif

    private event Action OnInterstitialAdHiddenEvent;
    private event Action OnRewardedAdSuccessfulEvent;
    private event Action OnRewardedAdFailedEvent;
    private event Action OnRewardedAdHiddenEvent;
    
    private AdConfig _adConfig;
    private float _lastInterstitialTime;
    private Action _onReady;
    
    private const string INT_WAIT_ROUTINE_KEY = "int_wait_routine";
    private const string RW_WAIT_ROUTINE_KEY = "rw_wait_routine";
    private const float WAIT_TIME_OUT_SECONDS = 3f;

    public IAdProvider CreateSelf()
    {
        return new ApplovinAdProvider();
    }

    public void Initialize(Action onReady)
    {
        #if AdManager_Enabled
        _adConfig = GameInstaller.Instance.SystemLocator.RemoteConfigManager.GetObject<AdConfig>(RemoteConfigTypes.AdConfig);
#else
        _adConfig = new AdConfig();
#endif
        MaxSdk.SetSdkKey("z_nBSimgy8WrJ1ahrToTwe_h4oDwXD0hCybCGmN9MmELRZhv7dNkZJFaLNWc3s5EdivK8kCO9OTZT07iPgrueD");
        MaxSdk.SetUserId("USER_ID");
        MaxSdk.InitializeSdk();
        _onReady = onReady;
        if (GameInstaller.Instance.SystemLocator.DataManager.GetData<Dictionary<string, ulong>>(GameDataType.State)
                ["CurrentLevel"] == 1)
        {
            _lastInterstitialTime = Time.time + _adConfig.FirstInterstitialAdDelay;
        }
        else
            _lastInterstitialTime = Time.time + _adConfig.GameOpenInterstitialAdDelay;

        MaxSdkCallbacks.OnSdkInitializedEvent += OnMaxSdkInitialized;
    }

    private void OnMaxSdkInitialized(MaxSdkBase.SdkConfiguration obj)
    {
        InitializeInterstitialAds();
        InitializeRewardedAds();
        InitializeBannerAds();

        LoadInterstitialAd();
        LoadRewardedAd();
        LoadBannerAd();
        _onReady?.Invoke();
    }

    #region Interstitial

    private void InitializeInterstitialAds()
    {
        // Attach callback
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterstitialAdRevenuePaidEvent;

        // Load the first interstitial
        LoadInterstitialAd();
    }

    private void LoadInterstitialAd()
    {
        MaxSdk.LoadInterstitial(adUnitIdInter);
    }

    private IEnumerator InterstitialAdWaitRoutine(Action action = null)
    {
        LoadInterstitialAd();
        var timer = WAIT_TIME_OUT_SECONDS;
        while (timer > 0 && !MaxSdk.IsInterstitialReady(adUnitIdInter))
        {
            yield return new WaitForSeconds(0.1f);
            timer -= 0.1f;
            Debug.Log($"[{this}]Interstitial ad is not ready. Waiting...");
        }

        action?.Invoke();
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
            Debug.Log($"[{this}]Interstitial ad is not ready");
            return;
        }

        OnInterstitialAdHiddenEvent = onClosed;
        if (MaxSdk.IsInterstitialReady(adUnitIdInter))
        {
            MaxSdk.ShowInterstitial(adUnitIdInter);
        }
        else
        {
            CoroutineController.StartCoroutine(INT_WAIT_ROUTINE_KEY, InterstitialAdWaitRoutine(() =>
            {
                if (MaxSdk.IsInterstitialReady(adUnitIdInter))
                {
                    MaxSdk.ShowInterstitial(adUnitIdInter);
                }
                else
                {
                    onClosed?.Invoke();
                    OnInterstitialAdHiddenEvent = null;
                }
            }));
        }
    }

    public bool CanShowInterstitial()
    {
        return _lastInterstitialTime < Time.time && _adConfig.FirstInterstitialLevel <=
            (int)GameInstaller.Instance.SystemLocator.DataManager.GetData<Dictionary<string, ulong>>(GameDataType.State)
                ["CurrentLevel"];
    }

    #region Interstitial Ad Callbacks

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
    }

    private void OnInterstitialAdRevenuePaidEvent(string arg1, MaxSdkBase.AdInfo arg2)
    {
    }

    private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
    }

    private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // interstitialAdShownEvent?.Invoke();
    }

    private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo,
        MaxSdkBase.AdInfo adInfo)
    {
        LoadInterstitialAd();
    }

    private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
    }

    private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        LoadInterstitialAd();
        _lastInterstitialTime = Time.time + _adConfig.InterstitialInterval;
        OnInterstitialAdHiddenEvent?.Invoke();
    }

    #endregion

    #endregion

    #region Rewarded

    private void InitializeRewardedAds()
    {
        // Attach callback
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHidden;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
        // Load the first rewarded ad
        LoadRewardedAd();
    }

    private void LoadRewardedAd()
    {
        MaxSdk.LoadRewardedAd(adUnitIdRewarded);
    }

    public void ShowRewarded(Action onCompleted, Action onFailed = null)
    {
        if (!_adConfig.RewardedAdEnabled)
        {
            onCompleted?.Invoke();
            _lastInterstitialTime = Time.time + _adConfig.RewardedAdCooldown;
            return;
        }
        
        OnRewardedAdSuccessfulEvent = onCompleted;
        OnRewardedAdFailedEvent = onFailed;
        OnRewardedAdHiddenEvent = onFailed;
        
        if (MaxSdk.IsRewardedAdReady(adUnitIdRewarded))
        {
            MaxSdk.ShowRewardedAd(adUnitIdRewarded);
        }
        else
        {
            CoroutineController.StartCoroutine(RW_WAIT_ROUTINE_KEY, RewardedAdWaitRoutine(() =>
            {
                if (MaxSdk.IsRewardedAdReady(adUnitIdRewarded))
                {
                    MaxSdk.ShowRewardedAd(adUnitIdRewarded);
                }
                else
                {
                    onFailed?.Invoke();
                    OnRewardedAdSuccessfulEvent = null;
                    OnRewardedAdFailedEvent = null;
                    OnRewardedAdHiddenEvent = null;
                }
            }));
        }
    }

    private IEnumerator RewardedAdWaitRoutine(Action action = null)
    {
        LoadRewardedAd();
        var timer = WAIT_TIME_OUT_SECONDS;
        while (timer > 0 && !MaxSdk.IsRewardedAdReady(adUnitIdRewarded))
        {
            yield return new WaitForSeconds(0.1f);

            timer -= 0.1f;
            Debug.Log($"[{this}] Rewarded ad is not ready. Waiting...");
        }

        action?.Invoke();
    }

    #region Rewarded Ad Callbacks
    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo){}
    

    private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // SendRewardedAdFailedLoadingEvent
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        //SendRewardedShownEvent
    }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        OnRewardedAdFailedEvent?.Invoke();
        OnRewardedAdFailedEvent = null;
        LoadRewardedAd();
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        //SendRewardedClickedEvent
    }

    private void OnRewardedAdHidden(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        LoadRewardedAd();
        OnRewardedAdHiddenEvent?.Invoke();
        OnRewardedAdHiddenEvent = null;
        _lastInterstitialTime = Time.time + _adConfig.RewardedAdCooldown;
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        OnRewardedAdSuccessfulEvent?.Invoke();
        OnRewardedAdSuccessfulEvent = null;
        //  SendRewardedClickedEvent
    }

    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Ad revenue paid. Use this callback to track user revenue.
    }
    #endregion

    #endregion

    #region Banner

    private void InitializeBannerAds()
    {
        MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdLoadFailedEvent;
        MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;
        MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnBannerAdExpandedEvent;
        MaxSdkCallbacks.Banner.OnAdCollapsedEvent += OnBannerAdCollapsedEvent;

        MaxSdk.CreateBanner(adUnitIdBanner, MaxSdkBase.BannerPosition.BottomCenter);
        MaxSdk.SetBannerBackgroundColor(adUnitIdBanner, Color.black);
    }

    private void LoadBannerAd()
    {
        MaxSdk.LoadBanner(adUnitIdBanner);
    }

    public void ShowBanner()
    {
        if (!_adConfig.BannerAdEnabled)
        {
            Debug.Log($"[{this}]Banner ad is not enabled");
            return;
        }

        if (!MaxSdk.IsInitialized())
        {
            return;
        }

        MaxSdk.ShowBanner(adUnitIdBanner);
        MaxSdk.StartBannerAutoRefresh(adUnitIdBanner);
        // banner shown
    }

    public void HideBanner()
    {
        if (!MaxSdk.IsInitialized())
        {
            return;
        }

        MaxSdk.HideBanner(adUnitIdBanner);
        // banner hidden
    }

    #region Banner Ad Callbacks

    private void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        if (_adConfig.BannerAdEnabled)
        {
            ShowBanner();
        }
        else
        {
            HideBanner();
        }

        Debug.Log($"[{this}]Banner loaded");
    }

    private void OnBannerAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
    }


    private void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
    }

    private void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
    }

    private void OnBannerAdExpandedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
    }

    private void OnBannerAdCollapsedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
    }

    #endregion

    #endregion
    
}
#endif