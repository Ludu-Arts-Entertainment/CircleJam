using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
//using Hellmade.Net;
using PlayFab;
using UnityEngine;
using Random = UnityEngine.Random;

public class LoginManager : IManager, ITrackListener
{
    // public static UnityAction<IState> StateChangedAction;
    public readonly HashSet<string> CountryCodes = new()
    {
        "ph",
        "us",
        "gb",
        "tr",
    };

    public bool IsLoggedIn { get; set; } = false;
    public string UserId { get; set; }
    public string PlayFabId { get; set; }
    public string CountryCode { get; set; }

    public string GetCountryCodeExtension(string countryCode) =>
        CountryCodes.Contains(countryCode) ? countryCode : "default";

    public string GetGlobalCountryCodeExtension() => "world";
    public string GetWeeklyExtension() => "weekly";
    public string GetLevelExtension() => "default";

    private Dictionary<LoginProviderEnums, ILoginProvider> _loginProviders;
    private DataManager _dataManager;
    private PlayFabLinkingHelper _playFabLinkingHelper;

    public IManager CreateSelf()
    {
        return new LoginManager();
    }


    public async void Initialize(GameInstaller gameInstaller, Action onReady)
    {
        _dataManager = gameInstaller.SystemLocator.DataManager;
        _loginProviders = new Dictionary<LoginProviderEnums, ILoginProvider>();
        _playFabLinkingHelper = new PlayFabLinkingHelper();

        var selectedLoginProviderTypesList = gameInstaller.Customizer.LoginProvider.GetFlags().ToList();

        if (selectedLoginProviderTypesList == null || selectedLoginProviderTypesList.Count < 1)
            return; // If the list is null, then we don't need to initialize anything

        if (selectedLoginProviderTypesList.Contains(LoginProviderEnums.None))
            return; // If LoginProviderEnums.None is in the list, then we don't need to initialize anything

        // Initialize available login providers, which are selected in the GameInstaller
        foreach (var loginProviderType in selectedLoginProviderTypesList)
        {
            var providerInstance = LoginProviderFactory.Create(loginProviderType);

            if (providerInstance == null)
            {
                throw new NullReferenceException(
                    $"Provider instance is null. You must check in the LoginProviderFactory class if there is exist, if not add the new provider to the switch statement");
            }

            providerInstance.Initialize(this, () => { _loginProviders.Add(loginProviderType, providerInstance); });
            await UniTask.WaitUntil(() => _loginProviders.ContainsKey(loginProviderType));
        }

        // Following conditional statement is for the case that we're not ready for registering user yet
        // Because we need to wait until end of onboarding or passing some level or session count 
        if (!IsLoginThresholdPassed())
        {
            Debug.Log(
                $"LoginManager was initialized but level threshold is not passed yet. Current level is {_dataManager.GetData(GameDataType.State)}");
            TrackingService.Track(TrackType.LoginRequirementsMet, this);
            onReady?.Invoke();
            return;
        }

        // Try auto login if user has been logged in before 
        await TryAllLogin(onReady);

        // Fetch all data from PlayFab
    }

    #region TrackListener Methods

    public async void OnTrackTriggered(params object[] args)
    {
        if (args[0] is not true)
            return;
        if (_loginProviders.First().Value.HaveBeenLoggedInBefore())
            return;

        await TryAllLogin(null);
    }

    #endregion

    public async UniTask TryLogin(LoginProviderEnums providerType, bool ignoreFirstLogin, Action onReady)
    {
        var loginProvider = GetProvider(providerType);
        if (loginProvider is null)
            return;

        // var internetStatus = EazyNetChecker.Status;
        // if (internetStatus == NetStatus.NoDNSConnection) return;

        if (loginProvider.HaveBeenLoggedInBefore() || ignoreFirstLogin)
        {
            // Check if login provider is registered, then login, otherwise register
            var asyncLoginTask = loginProvider.IsRegistered()
                ? loginProvider.Login(onReady)
                : loginProvider.Register(onReady);
            await UniTask.WhenAll(asyncLoginTask);
            var stateData = _dataManager.GetData<Dictionary<string, int>>(GameDataType.State);

            if (stateData.TryAdd(PersistentKeys.IsFirstLogin, 1))
            {
                _dataManager.SetData(GameDataType.State, stateData);
            }
        }
    }

    private async UniTask TryAllLogin(Action onReady)
    {
        List<UniTask> asyncLoginTasks = new List<UniTask>();
        // Check if login provider has been logged-in before, then login
        foreach (var providerType in _loginProviders.Keys)
        {
            asyncLoginTasks.Add(TryLogin(providerType, providerType == LoginProviderEnums.PlayFabLoginProvider, null));
        }

        // Wait until all login tasks are completed
        await UniTask.WhenAll(asyncLoginTasks);

        onReady?.Invoke();
    }

    public void LogOut()
    {
        foreach (var loginProvider in _loginProviders.Values)
        {
            loginProvider.LogOut();
        }
    }

    private bool IsLoginThresholdPassed()
    {
        return true;
        var loginStatusData =
            GameInstaller.Instance.SystemLocator.DataManager.GetData<LoginStatusModel>(GameDataType
                .LoginStatusData);
        return loginStatusData.IsLoginRequirementsMet;
    }

    public bool IsReady()
    {
        return _loginProviders != null && _loginProviders.Count > 0;
    }

    public void Dispose()
    {
        _loginProviders = null;
        // TODO: dispose providers
    }

    public ILoginProvider GetProvider(LoginProviderEnums loginProviderType)
    {
        if (_loginProviders == null)
            throw new NullReferenceException("There isnt any initialized login provider.");

        if (!_loginProviders.ContainsKey(loginProviderType))
            throw new KeyNotFoundException("This login provider type is not found in the initialized login providers.");

        return _loginProviders[loginProviderType];
    }

    // TODO : Following methods may move to somewhere else
    public async UniTask UpdateUsername(string username)
    {
        var status = true;//EazyNetChecker.Status;
        var connected = true;//status == NetStatus.Connected;
        // if it is not logged in or internet connection is not stable then save it to local
        if (!connected || !PlayFabClientAPI.IsClientLoggedIn())
        {
            var profileState = _dataManager.GetData<ProfileModel>(GameDataType.ProfileData);
            profileState.Name.ShortUsername = username;
            profileState.Name.UniqueNumber = Random.Range(100000, 999999).ToString();
            var profileSummaryData = _dataManager.GetData<ProfileSummaryData>(GameDataType.ProfileSummaryData);
            profileSummaryData.profile.Name = profileState.Name;
            _dataManager.SetData(GameDataType.ProfileSummaryData, profileSummaryData);
            _dataManager.SetData(GameDataType.ProfileData, profileState);
            _dataManager.SaveData();
            return;
        }
#if PlayFabSdk_Enabled
            await PlayFabHelper.UpdateUsername(username);
#endif

        GameInstaller.Instance.SystemLocator.DataManager.SaveRemoteData();
    }

    public async UniTask UpdateAvatar(int avatarIndex)
    {
        if (!PlayFabClientAPI.IsClientLoggedIn())
        {
            var profileState = _dataManager.GetData<ProfileModel>(GameDataType.ProfileData);
            profileState.AvatarIndex = avatarIndex.ToString();
            _dataManager.SetData(GameDataType.ProfileData, profileState);
            _dataManager.SaveData();
            return;
        }
#if PlayFabSdk_Enabled
        await PlayFabHelper.UpdateAvatar(avatarIndex);
#endif
        GameInstaller.Instance.SystemLocator.DataManager.SaveRemoteData();
    }
    
    #region Linking Social Accounts Methods
#if FacebookSdk_Enabled && PlayFabSdk_Enabled

    public async UniTask<bool> LinkFacebookAccountToPlayFabAsync(bool forceLink)
    {
        // Check if the user is logged in to PlayFab
        if (!PlayFabClientAPI.IsClientLoggedIn())
            return false;

        // Check if internet connection is available
        // if (EazyNetChecker.Status == NetStatus.NoDNSConnection)
        //     return false;

        // Get the FacebookLoginProvider from the LoginManager
        var facebookLoginProvider = GetProvider(LoginProviderEnums.FacebookLoginProvider) as FacebookLoginProvider;
        if (facebookLoginProvider == null)
            return false;

        var isLoggedInResult = await facebookLoginProvider.SignInWithFacebookAsync();
        if (isLoggedInResult is false)
        {
            return false;
        }

        // Start the Linking process with appleIDCredential to PlayFab
        var isLinked = await _playFabLinkingHelper.LinkFacebookAccountToPlayFab(forceLink: forceLink);

        if (isLinked)
        {
            var loginStatusData = _dataManager.GetData(GameDataType.LoginStatusData) as LoginStatusModel;
            loginStatusData.IsFacebookLoggedIn = true;
            _dataManager.SetData(GameDataType.LoginStatusData, loginStatusData);

            // TODO : Save facebook token if its needed
        }

        return isLinked;
    }

    public async UniTask UnlinkFacebookAccountFromPlayFabAsync()
    {
        await _playFabLinkingHelper.UnlinkFacebookAccountFromPlayFabAsync();
    }
#endif
    
#if AppleAuth_Enabled && PlayFabSdk_Enabled
    public async UniTask<bool> LinkAppleAccountToPlayFabAsync(bool forceLink)
    {
        // Check if the user is logged in to PlayFab
        if (!PlayFabClientAPI.IsClientLoggedIn())
            return false;

        // Check if internet connection is available
        // if (EazyNetChecker.Status == NetStatus.NoDNSConnection)
        //     return false;

        // Get the AppleLoginProvider from the LoginManager
        var appleLoginProvider = GetProvider(LoginProviderEnums.AppleLoginProvider) as AppleLoginProvider;
        if (appleLoginProvider == null)
            return false;

        // Start the Apple Sign In process
        var appleIDCredential = await appleLoginProvider.SignInWithAppleAsync();
        if (appleIDCredential is null)
        {
            return false;
        }

        // Start the Linking process with appleIDCredential to PlayFab
        var isLinked =
            await _playFabLinkingHelper.LinkAppleAccountToPlayFabAsync(appleIdCredential: appleIDCredential,
                forceLink: forceLink);

        if (isLinked)
        {
            var loginStatusData = _dataManager.GetData(GameDataType.LoginStatusData) as LoginStatusModel;
            loginStatusData.IsAppleLoggedIn = true;
            _dataManager.SetData(GameDataType.LoginStatusData, loginStatusData);

            // TODO : Save appleIDCredential if its needed
        }

        return isLinked;
    }

    public async UniTask UnlinkAppleAccountFromPlayFabAsync()
    {
        await _playFabLinkingHelper.UnlinkAppleAccountFromPlayFabAsync();
    }
#endif

    #endregion Linking Social Accounts Methods
}

public partial class TrackType
{
    public static string LoginRequirementsMet => "LoginRequirementsMet";
}