#if FacebookSdk_Enabled

using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
// Import statements introduce all the necessary classes for this example.
using Facebook.Unity;
using PlayFab;
using PlayFab.ClientModels;
using LoginResult = PlayFab.ClientModels.LoginResult;
public class FacebookLoginProvider : ILoginProvider
{
    private string _message;
    private bool _isLoginCompleted = false;
    private DataManager _dataManager;
    private LoginManager _loginManager;
    private LoginStatusModel _loginStatusData;
    
    #region Interface Overrides

    public ILoginProvider CreateSelf()
    {
        return new FacebookLoginProvider();
    }

    public async void Initialize(LoginManager loginManager, Action onReady)
    {
        await UniTask.WaitUntil(()=>GameInstaller.Instance.ManagerDictionary.ContainsKey(ManagerEnums.DataManager));
        _dataManager = GameInstaller.Instance.SystemLocator.DataManager;
        _loginManager = loginManager;
        _loginStatusData = _dataManager.GetData(GameDataType.LoginStatusData) as LoginStatusModel;
        
        await UniTask.WaitUntil(() => FB.IsInitialized);
        
        //Login(onReady);
        onReady?.Invoke();
        Debug.Log($"{nameof(FacebookLoginProvider)}.{nameof(Initialize)} was called");
    }

    public bool IsRegistered()
    {
        return FB.IsLoggedIn;
    }

    public bool HaveBeenLoggedInBefore()
    {
        return _dataManager.GetData<LoginStatusModel>(GameDataType.LoginStatusData).IsFacebookLoggedIn;
    }

    public async UniTask Login(Action onReady)
    {
        onReady?.Invoke();
        await UniTask.Yield();
        return;
        Debug.Log("Logging into Facebook...");
        
        // Once Facebook SDK is initialized, if we are logged in, we log out to demonstrate the entire authentication cycle.
        if (FB.IsLoggedIn)
            FB.LogOut();
        
        // We invoke basic login procedure and pass in the callback to process the result
        FB.LogInWithReadPermissions(null, OnFacebookLoggedIn);
        
        await UniTask.WaitUntil(() => _isLoginCompleted);
        
    }

    public async UniTask Register(Action onReady)
    {
        onReady?.Invoke();
        await UniTask.Yield();
        // await Login(onReady);
    }

    public async UniTask LogOut()
    {
        _loginStatusData.IsFacebookLoggedIn = false;
    }

    #endregion
    
    [Obsolete]
    private void OnFacebookLoggedIn(ILoginResult result)
    {
        // If result has no errors, it means we have authenticated in Facebook successfully
        if (FB.IsLoggedIn)
        {
            Debug.Log("Facebook Auth Complete! Access Token: " + AccessToken.CurrentAccessToken.TokenString + "\nLogging into PlayFab...");

            /*
             * We proceed with making a call to PlayFab API. We pass in current Facebook AccessToken and let it create
             * and account using CreateAccount flag set to true. We also pass the callback for Success and Failure results
             */
            
            // PlayFabClientAPI.LoginWithFacebook(new LoginWithFacebookRequest { CreateAccount = true, AccessToken = AccessToken.CurrentAccessToken.TokenString},
            //     OnPlayfabFacebookAuthComplete, OnPlayfabFacebookAuthFailed);
        }
        else
        {
            // If Facebook authentication failed, we stop the cycle with the message
            Debug.LogError("Facebook Auth Failed: " + result.Error + "\n" + result.RawResult);
        }
    }

    private void OnPlayfabFacebookLinkComplete(LinkFacebookAccountResult result)
    {
        Debug.Log("PlayFab Facebook Auth Complete. Session ticket: " + result);
        _isLoginCompleted = true;
    }
    private void OnPlayfabFacebookLinkFailed(PlayFabError error)
    {
        Debug.LogError("PlayFab Facebook Auth Failed: " + error.GenerateErrorReport());
        _isLoginCompleted = true;
    }

    // When processing both results, we just set the message, explaining what's going on.
    private void OnPlayfabFacebookAuthComplete(LoginResult result)
    {
        Debug.Log("PlayFab Facebook Auth Complete. Session ticket: " + result.SessionTicket);
    }

    private void OnPlayfabFacebookAuthFailed(PlayFabError error)
    {
        Debug.LogError("PlayFab Facebook Auth Failed: " + error.GenerateErrorReport());
        _isLoginCompleted = true;
    }

    public void SetMessage(string message, bool error = false)
    {
        _message = message;
        if (error)
            Debug.LogError(_message);
        else
            Debug.Log(_message);
    }

    public void OnGUI()
    {
        var style = new GUIStyle { fontSize = 40, normal = new GUIStyleState { textColor = Color.white }, alignment = TextAnchor.MiddleCenter, wordWrap = true };
        var area = new Rect(0,0,Screen.width,Screen.height);
        GUI.Label(area, _message,style);
    }

    public async UniTask<bool> SignInWithFacebookAsync()
    {
        // Once Facebook SDK is initialized, if we are logged in, we log out to demonstrate the entire authentication cycle.
        if (FB.IsLoggedIn)
            FB.LogOut();
        
        bool isResponseReceived = false;
        bool isLoggedIn = false;

        FB.LogInWithReadPermissions(null, OnLogInWithReadPermissions);
        
        void OnLogInWithReadPermissions(ILoginResult result)
        {
            isResponseReceived = true;
               
            if (FB.IsLoggedIn)
            {
                Debug.Log("Facebook Auth Complete! Access Token: " + AccessToken.CurrentAccessToken.TokenString + "\nLogging into PlayFab...");
                isLoggedIn = true;
            }
            else
            {
                // If Facebook authentication failed, we stop the cycle with the message
                Debug.LogError("Facebook Auth Failed: " + result.Error + "\n" + result.RawResult);
                isLoggedIn = false;
            }
        }
        
        await UniTask.WaitUntil(() => isResponseReceived);
        
        return isLoggedIn;
    }
}
#endif
