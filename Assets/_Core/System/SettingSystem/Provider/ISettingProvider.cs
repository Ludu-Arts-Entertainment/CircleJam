using System;
public interface ISettingProvider
{
    ISettingProvider CreateSelf();
    void Initialize(Action onReady);
    void SetSetting(SettingType settingType, float value);
    void SetSetting(SettingType settingType, string value);
    void GetSetting<T>(SettingType settingType, out T value);
}
