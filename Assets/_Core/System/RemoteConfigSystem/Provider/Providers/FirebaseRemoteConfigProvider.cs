#if FirebaseRemoteConfig_Enabled
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using UnityEngine;


public class FirebaseRemoteConfigProvider : IRemoteConfigProvider
{
    private Dictionary<Type, object> _remoteConfigDictionary =
        new DefaultRemoteData().RemoteConfigDictionary;
    FirebaseRemoteConfig _remoteConfig;
    public IRemoteConfigProvider CreateSelf()
    {
        return new FirebaseRemoteConfigProvider();
    }
    private Action _onReady;
    public async void Initialize(Action onReady)
    {
        _onReady = onReady;
        _remoteConfig = FirebaseRemoteConfig.DefaultInstance;
        var configSettings = new ConfigSettings
        {
            MinimumFetchInternalInMilliseconds = 0,
            FetchTimeoutInMilliseconds = 1000 * 10 // 10 seconds
        };
        await _remoteConfig.SetConfigSettingsAsync(configSettings);
        _remoteConfig.OnConfigUpdateListener += ConfigUpdateListenerEventHandler;
        await FetchDataAsync();
    }

    private Task FetchDataAsync() {
        Debug.Log($"[{this}]Fetching data...");
        Task fetchTask =
            FirebaseRemoteConfig.DefaultInstance.FetchAsync(
                TimeSpan.Zero);
        return fetchTask.ContinueWithOnMainThread(FetchComplete);
    }
    private void FetchComplete(Task fetchTask) {
        
        if (!fetchTask.IsCompleted) {
            Debug.LogError($"[{this}Retrieval hasn't finished.");
            return;
        }

        var info = _remoteConfig.Info;
        if(info.LastFetchStatus != LastFetchStatus.Success) {
            Debug.LogError($"[{this} {nameof(FetchComplete)} was unsuccessful\n{nameof(info.LastFetchStatus)}: {info.LastFetchStatus}");
            _onReady.Invoke();
            return;
        }

        // Fetch successful. Parameter values must be activated to use.
        _remoteConfig.ActivateAsync()
            .ContinueWithOnMainThread(
                task => {
                    Debug.Log($"[{this} Remote data loaded and ready for use. Last fetch time {info.FetchTime}.");
                    _onReady.Invoke();
                });
    }
    void ConfigUpdateListenerEventHandler(
        object sender, ConfigUpdateEventArgs args) {
        if (args.Error != RemoteConfigError.None) {
            Debug.Log(String.Format("Error occurred while listening: {0}", args.Error));
            return;
        }

        Debug.Log("Updated keys: " + string.Join(", ", args.UpdatedKeys));
        // Activate all fetched values and then display a welcome message.
        _remoteConfig.ActivateAsync().ContinueWithOnMainThread(
            task => {
                Debug.Log($"[{this} Remote data was updated.Last fetch time {args.UpdatedKeys}.");
            });
    }
    
    public T GetObject<T>(T defaultValue) where T: IConfig
    {
        if (!_remoteConfig.Keys.Contains(typeof(T).Name))
        {
            if (_remoteConfigDictionary.TryGetValue(typeof(T), out var value))
            {
                return (T)value;
            }
            return defaultValue ?? (T)Activator.CreateInstance(typeof(T));
        }
        try
        {
            return JsonHelper.FromJson<T>(_remoteConfig.GetValue(typeof(T).Name).StringValue);
        }
        catch (Exception e)
        {
            return _remoteConfigDictionary.TryGetValue(typeof(T), out var value) ? (T)value : defaultValue;
        }
    }
}
#endif
