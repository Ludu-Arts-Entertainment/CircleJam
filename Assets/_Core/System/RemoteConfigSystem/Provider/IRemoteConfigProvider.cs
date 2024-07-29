using System;

public interface IRemoteConfigProvider
{
    IRemoteConfigProvider CreateSelf();
    void Initialize(Action onReady);
    T GetObject<T>(T defaultValue) where T: IConfig;
}