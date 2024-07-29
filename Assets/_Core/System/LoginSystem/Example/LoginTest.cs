using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;

public class LoginTest : MonoBehaviour
    {
        private bool _readyToTest = false;
        
        LoginManager _loginManager;
        DataManager _dataManager;
        
        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        async void Awake()
        {
            await UniTask.WaitUntil(()=> GameInstaller.Instance.ManagerDictionary.ContainsKey(ManagerEnums.EventManager));
            GameInstaller.Instance.SystemLocator.EventManager.Subscribe<Events.OnGameReadyToStart>(OnGameReadyToStart);
        }

        private void OnGameReadyToStart(Events.OnGameReadyToStart start)
        {
            _loginManager ??= GameInstaller.Instance.SystemLocator.LoginManager;
            _dataManager ??= GameInstaller.Instance.SystemLocator.DataManager;
            Debug.Log($"{_loginManager} and {_dataManager} were cached and ready to use!");
            _readyToTest = true;
        }

        [EnableIf("_readyToTest")][NaughtyAttributes.Button]
        public void LoginPlayFab()
        {
            _loginManager.TryLogin(LoginProviderEnums.PlayFabLoginProvider, true,null);
            Debug.Log($"PlayFab Login Status: {_loginManager.GetProvider(LoginProviderEnums.PlayFabLoginProvider).IsRegistered()}");
        }

        [EnableIf("_readyToTest")][NaughtyAttributes.Button]
        public async void UpdateAvatarIndex()
        {
            var profileState = _dataManager.GetData<ProfileModel>(GameDataType.ProfileData);
            var currentAvatarIndex = Convert.ToInt32(profileState.AvatarIndex); 

            Convert.ToInt32(currentAvatarIndex);
            
            await _loginManager.UpdateAvatar(currentAvatarIndex);
        }
        
        [EnableIf("_readyToTest")][NaughtyAttributes.Button]
        public async void UpdateUsername()
        {
            var profileState = _dataManager.GetData<ProfileModel>(GameDataType.ProfileData);
            var currentProfileName = profileState.Name.ShortUsername;
            await _loginManager.UpdateUsername(currentProfileName);
        }
        
        [EnableIf("_readyToTest")][NaughtyAttributes.Button]
        public async void LoginFacebook()
        {
            // var status = await PlayFabHelper.LinkFacebookAccountToPlayFab(false);
            _loginManager.GetProvider(LoginProviderEnums.FacebookLoginProvider).Login(OnReady);
            // _loginManager.TryLogin(LoginProviderEnums.FacebookLoginProvider, true,null);
            void OnReady()
            {
                Debug.Log("Facebook Login is ready!");
            }
        } 
    }