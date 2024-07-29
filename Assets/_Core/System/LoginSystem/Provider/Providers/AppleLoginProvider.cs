#if AppleAuth_Enabled

using System;
using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AppleAuth.Native;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AppleLoginProvider : ILoginProvider
{
    private string _customUserId;
    private DataManager _dataManager;
    private LoginManager _loginManager;
    private LoginStatusModel _loginStatusData;
    
    private IAppleAuthManager _appleAuthManager;
    private PayloadDeserializer _deserializer;
        
    public ILoginProvider CreateSelf()
    {
        return new AppleLoginProvider();
    }

    public async void Initialize(LoginManager loginManager, Action onReady)
    {
        await UniTask.WaitUntil(()=>GameInstaller.Instance.ManagerDictionary.ContainsKey(ManagerEnums.DataManager));
        
        _dataManager = GameInstaller.Instance.SystemLocator.DataManager;
        _loginManager = loginManager;
        _loginStatusData = _dataManager.GetData(GameDataType.LoginStatusData) as LoginStatusModel;
        
        GameInstaller.Instance.OnUpdate += Update;
      
        // If the current platform is supported
        if (AppleAuthManager.IsCurrentPlatformSupported)
        {
            // Creates a default JSON deserializer, to transform JSON Native responses to C# instances
            var deserializer = new PayloadDeserializer();
            Debug.Log($"deserializer is null: {deserializer == null}");
            // Creates an Apple Authentication manager with the deserializer
            this._appleAuthManager = new AppleAuthManager(deserializer);
            Debug.Log($"deserializer is null: {_appleAuthManager == null}");
        }
        
        Debug.Log($"{nameof(AppleLoginProvider)}.{nameof(Initialize)} was called");

        InitializeAppleLoginMenu();
        
        onReady?.Invoke();
    }

    private void Update()
    {
        if (_appleAuthManager != null)
        {
            _appleAuthManager.Update();
        }
    }

    private void InitializeAppleLoginMenu()
    {
        // Check if the current platform supports Sign In With Apple
        if (this._appleAuthManager == null)
        {
            return;
        }
        
        // If at any point we receive a credentials revoked notification, we delete the stored User ID, and go back to login
        this._appleAuthManager.SetCredentialsRevokedCallback(result =>
        {
            Debug.Log("Received revoked callback " + result);
            PlayerPrefs.DeleteKey(PersistentKeys.Authentication.AppleUserIdKey);
        });

        // If we have an Apple User Id available, get the credential status for it
        if (PlayerPrefs.HasKey(PersistentKeys.Authentication.AppleUserIdKey))
        {
            var storedAppleUserId = PlayerPrefs.GetString(PersistentKeys.Authentication.AppleUserIdKey);
            this.CheckCredentialStatusForUserId(storedAppleUserId);
        }
        // If we do not have an stored Apple User Id, attempt a quick login
        else
        {
            this.AttemptQuickLogin();
        }
    }

    public bool IsRegistered()
    {
        return _dataManager.GetData<ProfileModel>(GameDataType.ProfileData).UserId != string.Empty;
    }

    public bool HaveBeenLoggedInBefore()
    {
        return _loginStatusData.IsAppleLoggedIn;
    }

    public async UniTask Login(Action onReady)
    {
        await UniTask.Yield();
    }

    public async UniTask Register(Action onReady)
    {
        await UniTask.Yield();
    }

    public async UniTask LogOut()
    {
        _loginStatusData.IsAppleLoggedIn = false;
    }

    /// <summary>
    /// This method is used to sign in with apple
    /// </summary>
    /// <returns>IAppleIDCredential type value if the signed in is successful, otherwise null</returns>
    public async UniTask<IAppleIDCredential> SignInWithAppleAsync()
    {
        if (!AppleAuthManager.IsCurrentPlatformSupported)
        {
            Debug.LogError("Sign in with Apple is not supported on this platform");
            return null;
        }
        
        bool isAppleLoginResponseReceived = false;
        IAppleIDCredential appleIdCredential = null;

        // Create apple authentication login arguments, we dont need full name or email
        var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);

        // Attempt to login with apple
        this._appleAuthManager.LoginWithAppleId(
            loginArgs,
            LoginWithAppleIdSuccessCallback,
            LoginWithAppleIdErrorCallback);

        void LoginWithAppleIdSuccessCallback(ICredential credential)
        {
            
            // Cache response as IAppleIDCredential
            appleIdCredential = credential as IAppleIDCredential;
            
            // If a sign in with apple succeeds, we should have obtained the credential with the user id, name, and email, save it
            PlayerPrefs.SetString(PersistentKeys.Authentication.AppleUserIdKey, appleIdCredential.User);
            
            // Set the response as received
            isAppleLoginResponseReceived = true;
        }
        
        void LoginWithAppleIdErrorCallback(IAppleError error)
        {
            // If there is an error, log it
            var authorizationErrorCode = error.GetAuthorizationErrorCode(); 
            
            Debug.LogWarning("Sign in with Apple failed " + authorizationErrorCode.ToString() + " " + error.ToString());
            
            // Set the response as received
            isAppleLoginResponseReceived = true;
        }
        
        // Wait for the response to be received
        await UniTask.WaitUntil(() => isAppleLoginResponseReceived);
        
        // Return the result
        return appleIdCredential;
    }
    
    private void CheckCredentialStatusForUserId(string appleUserId)
    {
        // If there is an apple ID available, we should check the credential state
        this._appleAuthManager.GetCredentialState(
            appleUserId,
            state =>
            {
                switch (state)
                {
                    // If it's authorized, login with that user id
                    case CredentialState.Authorized:
                        return;
                    
                    // If it was revoked, or not found, we need a new sign in with apple attempt
                    // Discard previous apple user id
                    case CredentialState.Revoked:
                    case CredentialState.NotFound:
                        PlayerPrefs.DeleteKey(PersistentKeys.Authentication.AppleUserIdKey);
                        return;
                }
            },
            error =>
            {
                var authorizationErrorCode = error.GetAuthorizationErrorCode();
                Debug.LogWarning("Error while trying to get credential state " + authorizationErrorCode.ToString() + " " + error.ToString());
            });
    }
    
    private void AttemptQuickLogin()
    {
        var quickLoginArgs = new AppleAuthQuickLoginArgs();
        
        // Quick login should succeed if the credential was authorized before and not revoked
        this._appleAuthManager.QuickLogin(
            quickLoginArgs,
            credential =>
            {
                // If it's an Apple credential, save the user ID, for later logins
                var appleIdCredential = credential as IAppleIDCredential;
                if (appleIdCredential != null)
                {
                    PlayerPrefs.SetString(PersistentKeys.Authentication.AppleUserIdKey, credential.User);    
                }
            },
            error =>
            {
                // If Quick Login fails, we should show the normal sign in with apple menu, to allow for a normal Sign In with apple
                var authorizationErrorCode = error.GetAuthorizationErrorCode();
                Debug.LogWarning("Quick Login Failed " + authorizationErrorCode.ToString() + " " + error.ToString());
            });
    }
}
#endif
