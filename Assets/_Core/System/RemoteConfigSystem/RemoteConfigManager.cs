using System;
public class RemoteConfigManager : IManager
{
    private IRemoteConfigProvider _remoteConfigProvider; // if you want use firebase, add a "FirebaseRemoteConfig_Enabled" to symbols
    public IManager CreateSelf()
    {
        return new RemoteConfigManager();
    }

    public void Initialize(GameInstaller gameInstaller, Action onReady)
    {
        _remoteConfigProvider = RemoteConfigProviderFactory.Create(gameInstaller.Customizer.RemoteConfigProvider);
        _remoteConfigProvider.Initialize(onReady);
    }

    public bool IsReady()
    {
        return _remoteConfigProvider != null;
    }
    public T GetObject<T>(T defaultValue = default) where T: IConfig
    {
        return _remoteConfigProvider.GetObject(defaultValue);
    }
}
