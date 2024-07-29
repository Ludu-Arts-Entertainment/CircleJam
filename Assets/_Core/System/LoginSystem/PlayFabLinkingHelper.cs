using System;
#if AppleAuth_Enabled
using AppleAuth.Interfaces;
#endif
using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
#if FacebookSdk_Enabled
using FacebookAccessToken = Facebook.Unity.AccessToken;
#endif
public class PlayFabLinkingHelper
{
#if FacebookSdk_Enabled && PlayFabSdk_Enabled
    
    #region Facebook
    
    /// <summary>
    /// This method is used to link the facebook account to playfab if the facebook account was logged in
    /// </summary>
    /// <param name="forceLink">is boolean type. That is used for forced to link Fb account to this PlayFab user</param>
    /// <returns>boolean value true if the signed in is successful, otherwise false</returns>
    public async UniTask<bool> LinkFacebookAccountToPlayFab(bool forceLink = false)
    {
        if (!PlayFabClientAPI.IsClientLoggedIn())
            return false;

        bool isFacebookLinkResponseReceived = false;
        bool isLinked = false;

        // Create a request to link the facebook account to playfab
        LinkFacebookAccountRequest linkFacebookAccountRequest = new LinkFacebookAccountRequest()
        {
            AccessToken = FacebookAccessToken.CurrentAccessToken.TokenString,
            ForceLink = forceLink,
        };

        // Send the request to playfab
        PlayFabClientAPI.LinkFacebookAccount(linkFacebookAccountRequest,
            OnLinkFacebookAccountResponseCallback,
            OnLinkFacebookAccountErrorCallback);
        
        void OnLinkFacebookAccountResponseCallback(LinkFacebookAccountResult response)
        {
            isFacebookLinkResponseReceived = true;
            Debug.Log("Facebook account linked successfully: " + FacebookAccessToken.CurrentAccessToken.TokenString);
            isLinked = true;
        }

        void OnLinkFacebookAccountErrorCallback(PlayFabError playFabError)
        {
            isFacebookLinkResponseReceived = true;
            Debug.Log("Failed to link Facebook account: " + playFabError.GenerateErrorReport());

            if (playFabError.Error == PlayFabErrorCode.LinkedAccountAlreadyClaimed)
            {
                // TODO : Handle the case where the Facebook account is already linked to another PlayFab account
            }
            
            isLinked = false;
        }

        await UniTask.WaitUntil(() => isFacebookLinkResponseReceived);
        return isLinked;
    }

    public async UniTask UnlinkFacebookAccountFromPlayFabAsync()
    {
        if (!PlayFabClientAPI.IsClientLoggedIn())
            return;

        bool responseReceived = false;

        UnlinkFacebookAccountRequest request = new UnlinkFacebookAccountRequest();

        PlayFabClientAPI.UnlinkFacebookAccount(request,
            (response) =>
            {
                Debug.Log("Facebook account unlinked successfully");
                responseReceived = true;
            },
            (error) =>
            {
                Debug.Log("Failed to unlink Facebook account: " + error.ErrorMessage);
                responseReceived = true;
            });

        await UniTask.WaitUntil(() => responseReceived);
    }
    #endregion Facebook
#endif
#if AppleAuth_Enabled && PlayFabSdk_Enabled

    #region Apple

    /// <summary>
    /// This method is used to link the apple account to playfab if the apple account was logged in
    /// </summary>
    /// <param name="appleIdCredential">is IAppleIDCredential type parameter that is used for pass credential data</param>
    /// <param name="forceLink">is bool type parameter that is used for forcing apple account link to playfab</param>
    ///
    /// <returns>boolean value true if the signed in is successful, otherwise false</returns>
    ///
    /// <example>
    /// <code>
    /// var returnArg = await LinkAppleAccountToPlayFab(appleIdCredential: appleIDCredential, forceLink: false);
    /// </code>
    /// </example>
    /// 
    public async UniTask<bool> LinkAppleAccountToPlayFabAsync(IAppleIDCredential appleIdCredential, bool forceLink)
    {
        if (!PlayFabClientAPI.IsClientLoggedIn())
            return false;
        
        bool isAppleLinkResponseReceived = false;
        bool isLinked = false;
        
        var jwtFormattedIdentityToken = System.Text.Encoding.UTF8.GetString(appleIdCredential.IdentityToken);
        
        
        // Create a request to link the apple account to playfab
        LinkAppleRequest linkAppleRequest = new LinkAppleRequest
        {
            IdentityToken = jwtFormattedIdentityToken, 
            ForceLink = forceLink,
        };

        // Send the request to playfab
        PlayFabClientAPI.LinkApple(linkAppleRequest,
            OnLinkAppleResponseCallback,
            OnLinkAppleErrorCallback);
        
        void OnLinkAppleResponseCallback(EmptyResult emptyResult)
        {
            isAppleLinkResponseReceived = true;
            Debug.Log("Linking Apple account succeeded");
            isLinked = true;
            
            
        }

        void OnLinkAppleErrorCallback(PlayFabError playFabError)
        {
            isAppleLinkResponseReceived = true;
            
            if (playFabError.Error == PlayFabErrorCode.LinkedAccountAlreadyClaimed)
            {
                // TODO : Handle the case where the Facebook account is already linked to another PlayFab account
            }
            
            Debug.LogError("Linking Apple account failed with error: " + playFabError.GenerateErrorReport());
            isLinked = false;
        }

        // Wait until response is received
        await UniTask.WaitUntil(()=> isAppleLinkResponseReceived);
        return isLinked;
    }

    public async UniTask UnlinkAppleAccountFromPlayFabAsync()
    {
        if (!PlayFabClientAPI.IsClientLoggedIn())
            return;

        bool responseReceived = false;

        UnlinkAppleRequest unlinkAppleRequest = new UnlinkAppleRequest();

        PlayFabClientAPI.UnlinkApple(unlinkAppleRequest,
            (response) =>
            {
                Debug.Log("Apple account unlinked successfully");
                responseReceived = true;
            },
            (error) =>
            {
                Debug.Log("Failed to unlink Apple account: " + error.ErrorMessage);
                responseReceived = true;
            });

        await UniTask.WaitUntil(() => responseReceived);
    }
    
    #endregion Apple
#endif

}