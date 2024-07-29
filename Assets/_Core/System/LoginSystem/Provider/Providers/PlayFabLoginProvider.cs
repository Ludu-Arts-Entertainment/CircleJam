#if PlayFabSdk_Enabled
using System;
using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class PlayFabLoginProvider : ILoginProvider
{
    private string _customUserId;
    private DataManager _dataManager;
    private LoginManager _loginManager;
    private LoginStatusModel _loginStatusData;

    private readonly PlayerProfileViewConstraints _playerProfileViewConstraintsForRequest;
    private readonly GetPlayerCombinedInfoRequestParams _infoRequestParams;

    public PlayFabLoginProvider()
    {
        _playerProfileViewConstraintsForRequest = new PlayerProfileViewConstraints()
        {
            ShowAvatarUrl = true,
            ShowDisplayName = true,
            ShowLocations = true,
            ShowStatistics = true,
            ShowLastLogin = true,
            ShowTags = true
        };
        
        _infoRequestParams = new GetPlayerCombinedInfoRequestParams()
        {
            GetPlayerProfile = true,
            ProfileConstraints = _playerProfileViewConstraintsForRequest,
        };
    }
    
    public ILoginProvider CreateSelf()
    {
        return new PlayFabLoginProvider();
    }

    public async void Initialize(LoginManager loginManager, Action onReady)
    {
        await UniTask.WaitUntil(()=>GameInstaller.Instance.ManagerDictionary.ContainsKey(ManagerEnums.DataManager));
        _dataManager = GameInstaller.Instance.SystemLocator.DataManager;
        _loginManager = loginManager;

        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            // Please change the titleId below to your own titleId from PlayFab Game Manager.
            // If you have already set the value in the Editor Extensions, this can be skipped.
#if LoginManager_Enabled
            PlayFabSettings.staticSettings.TitleId = GameInstaller.Instance.SystemLocator.RemoteConfigManager.GetObject<LoginConfig>().PlayFabTitleId;
#endif
        }

        onReady?.Invoke();
        
        Debug.Log($"{nameof(PlayFabLoginProvider)}.{nameof(Initialize)} was called");
    }

    public bool IsRegistered()
    {
        return _dataManager.GetData<ProfileModel>(GameDataType.ProfileData).UserId != string.Empty;
    }

    public bool HaveBeenLoggedInBefore()
    {
        _loginStatusData = _dataManager.GetData<LoginStatusModel>(GameDataType.LoginStatusData); 
        return _loginStatusData.IsPlayFabLoggedIn;
    }

    #region Login

    public async UniTask Login(Action onReady)
    {
        bool responseReceived = false;
        
        _customUserId = _dataManager.GetData<ProfileModel>(GameDataType.ProfileData).UserId;
        // _customUserId = "59ebeef7-b266-42ba-888d-7f3eafc5a115-1702636186371.82";//_dataManager.GetData<ProfileModel>(GameDataType.ProfileData)[PersistentKeys.UserId];
        var loginWithCustomIDRequest = new LoginWithCustomIDRequest
        {
            CustomId = _customUserId,
            CreateAccount = false,
            InfoRequestParameters = _infoRequestParams,
        };

        PlayFabClientAPI.LoginWithCustomID(loginWithCustomIDRequest, OnLoginSuccess, OnLoginFailure);

        async void OnLoginSuccess(LoginResult result)
        {
            if (result is null)
            {
                throw new NullReferenceException("LoginResult is null");
            }
            
            await UniTask.WhenAll(_dataManager.GetPlayFabDataProvider().LoadAll(_dataManager.GetDataObject()));
            
            
            // Set the login flag
            PlayFabHelper.IsLoggedIn = true;
            _loginManager.IsLoggedIn = true;
            
            _loginStatusData.IsPlayFabLoggedIn = true;
            
            // Set the playfab id
            PlayFabHelper.PlayFabId = result.PlayFabId;
            _loginManager.PlayFabId = result.PlayFabId;
            PlayerPrefs.SetString(PersistentKeys.Authentication.PlayFabId, result.PlayFabId);
            
            // Get the profile data
            var profileData = _dataManager.GetData<ProfileModel>(GameDataType.ProfileData);
            var summaryData = _dataManager.GetData<ProfileSummaryData>(GameDataType.ProfileSummaryData);
            
            // Set user id
            PlayFabHelper.UserId = _customUserId;
            _loginManager.UserId = _customUserId;
            
            _dataManager.GetData<ProfileModel>(GameDataType.ProfileData).UserId = _customUserId;
            Debug.Log($"Player was logged in to PlayFab with id: \n{_customUserId}");

            // Set the last logged in country code
            var tmpCountryCode = result.InfoResultPayload?.PlayerProfile?.Locations[0]?.CountryCode.ToString().ToLower();
            tmpCountryCode ??= "default";

            PlayFabHelper.CountryCode = tmpCountryCode;
            _loginManager.CountryCode = tmpCountryCode;
            summaryData.profile.CountryCode = tmpCountryCode;
            profileData.CountryCode = tmpCountryCode;
            
            _dataManager.SetData(GameDataType.ProfileData, profileData);
            _dataManager.SetData(GameDataType.ProfileSummaryData, summaryData);
            _dataManager.SetData(GameDataType.LoginStatusData, _loginStatusData);
            _dataManager.SaveData();
            
            // TODO : We think SaveRemoteData is unnecessary 
            // _dataManager.SaveRemoteData();
            
            responseReceived = true;
        }
        
        void OnLoginFailure(PlayFabError error)
        {
            Debug.LogError("Something went wrong with your first API call.  :(");
            Debug.LogError("Here's some debug information:");
            Debug.LogError(error.GenerateErrorReport());
            
            responseReceived = true;
        }
        
        await UniTask.WaitUntil(() => responseReceived);
        onReady?.Invoke();
    }

    #endregion Login

    #region Register

    public async UniTask Register(Action onReady)
    {
        bool responseReceived = false;
        
        // Generate a unique user id
        _customUserId = GUIDHelper.GetUniqueGUID();
        
#if UNITY_EDITOR
        // _customUserId = SystemInfo.deviceUniqueIdentifier;
#endif
        
        var loginWithCustomIDRequest = new LoginWithCustomIDRequest
        {
            CustomId = _customUserId,
            CreateAccount = true,
            InfoRequestParameters = _infoRequestParams,
        };
        
        PlayFabClientAPI.LoginWithCustomID(loginWithCustomIDRequest, OnRegisterSuccess, OnRegisterFailure);

        async void OnRegisterSuccess(LoginResult result)
        {
            Debug.Log($"Congratulations, you made your first successful API call for {nameof(PlayFabLoginProvider)}!");
            
            // Set the login flag
            PlayFabHelper.IsLoggedIn = true;

            // Set user id
            PlayFabHelper.UserId = _customUserId;
            _dataManager.GetData<ProfileModel>(GameDataType.ProfileData).UserId = _customUserId;
            Debug.Log($"Player was logged in with id: \n{_customUserId}");

            // Set the playfab id
            PlayFabHelper.PlayFabId = result.PlayFabId;

            // Set the username if it is not generated yet.
            if (!IsUsernameGenerated())
            {
                await PlayFabHelper.UpdateUsername(GenerateUsername());
            }
            else
            {
                var profileData = _dataManager.GetData<ProfileModel>(GameDataType.ProfileData);
                await PlayFabHelper.UpdateUsername(profileData.Name.ShortUsername);
            }

            // Set the avatar index if it is not generated yet.
            await PlayFabHelper.UpdateAvatar(0);

            // After registration, call update the statistic to registering user to the leaderboard.
            // onReady += () =>
            // { };
            
            await Login(onReady);
            
            responseReceived = true;
        }
        
        void OnRegisterFailure(PlayFabError error)
        {
            Debug.LogError("Something went wrong with your first API call.  :(");
            Debug.LogError("Here's some debug information:");
            Debug.LogError(error.GenerateErrorReport());
            
            responseReceived = true;
        }
        
        await UniTask.WaitUntil(() => responseReceived);
    }

    public async UniTask LogOut()
    {
        await UniTask.WaitUntil(()=> !_loginStatusData.IsAppleLoggedIn && !_loginStatusData.IsFacebookLoggedIn && !_loginStatusData.IsGoogleLoggedIn);
        
        _loginStatusData.IsPlayFabLoggedIn = false;
        
        _dataManager.SetData(GameDataType.LoginStatusData, _loginStatusData);
        _dataManager.SaveData();
        
        await _dataManager.SaveRemoteData();
        
        PlayerPrefs.DeleteAll();
        Object.DestroyImmediate(GameInstaller.Instance.gameObject);
        SceneManager.LoadScene(0);
    }

    // TODO: This function needs to move leaderboard manager in some way

    private bool IsUsernameGenerated()
    {
        var profileState = _dataManager.GetData<ProfileModel>(GameDataType.ProfileData);
        return profileState.Name != null && profileState.Name.ShortUsername != string.Empty;
    }

    private string GenerateUsername()
    {
        return $"Guest"; //{UnityEngine.Random.Range(100000, 999999)}";
    }

    #endregion Register

    #region Login with Social Accounts Methods

    private async UniTask<LoginResult> LoginPlayfabWithAppleAsync(string appleIdentityToken)
    {
        bool loginResultReceived = false;
        LoginResult loginResult = null;
        
        LoginWithAppleRequest loginWithAppleRequest = new LoginWithAppleRequest()
        {
            CreateAccount = false,
            IdentityToken = appleIdentityToken,
            InfoRequestParameters = _infoRequestParams,
        };
        
        PlayFabClientAPI.LoginWithApple(loginWithAppleRequest, OnLoginWithAppleSuccess, OnLoginWithAppleFailure);
        
        void OnLoginWithAppleSuccess(LoginResult result)
        {
            loginResultReceived = true;
            loginResult = result;
        }
        
        void OnLoginWithAppleFailure(PlayFabError error)
        {
            Debug.LogError($"Something went wrong with your {nameof(PlayFabClientAPI.LoginWithApple)} API call.  :(");
            Debug.LogError("Here's some debug information:");
            Debug.LogError(error.GenerateErrorReport());
            
            loginResultReceived = true;
        }
        
        await UniTask.WaitUntil(() => loginResultReceived);
        return loginResult;
    }

    private async UniTask<LoginResult> LoginPlayfabWithFacebookAsync(string fbAccessToken)
    {
        bool loginResultReceived = false;
        LoginResult loginResult = null;
        
        LoginWithFacebookRequest loginWithFacebookRequest = new LoginWithFacebookRequest()
        {
            CreateAccount = false,
            AccessToken = fbAccessToken,
            InfoRequestParameters = _infoRequestParams,
        };
        
        PlayFabClientAPI.LoginWithFacebook(loginWithFacebookRequest, OnLoginWithFacebookSuccess, OnLoginWithFacebookFailure);
        
        void OnLoginWithFacebookSuccess(LoginResult result)
        {
            loginResultReceived = true;
            loginResult = result;
        }
        
        void OnLoginWithFacebookFailure(PlayFabError error)
        {
            Debug.LogError($"Something went wrong with your {nameof(PlayFabClientAPI.LoginWithFacebook)} API call.  :(");
            Debug.LogError("Here's some debug information:");
            Debug.LogError(error.GenerateErrorReport());
            
            loginResultReceived = true;
        }
        
        await UniTask.WaitUntil(() => loginResultReceived);
        return loginResult;
    }

    private async UniTask<LoginResult> LoginPlayfabWithGoogleAsync(string googleServerAuthCode)
    {
        bool loginResultReceived = false;
        LoginResult loginResult = null;
        
        LoginWithGoogleAccountRequest loginWithGoogleAccountRequest= new LoginWithGoogleAccountRequest()
        {
            CreateAccount = false,
            ServerAuthCode = googleServerAuthCode,
            InfoRequestParameters = _infoRequestParams,
        };
        
        PlayFabClientAPI.LoginWithGoogleAccount(loginWithGoogleAccountRequest, OnLoginWithGoogleSuccess, OnLoginWithGoogleFailure);
        
        void OnLoginWithGoogleSuccess(LoginResult result)
        {
            loginResultReceived = true;
            loginResult = result;
        }
        
        void OnLoginWithGoogleFailure(PlayFabError error)
        {
            Debug.LogError($"Something went wrong with your {nameof(PlayFabClientAPI.LoginWithGoogleAccount)} API call.  :(");
            Debug.LogError("Here's some debug information:");
            Debug.LogError(error.GenerateErrorReport());
            
            loginResultReceived = true;
        }
        
        await UniTask.WaitUntil(() => loginResultReceived);
        return loginResult;
    }

    #endregion Login with Social Accounts Methods
}
#endif
