using System;
using System.Collections.Generic;

public class BasicSettingProvider : ISettingProvider
{
    public ISettingProvider CreateSelf()
    {
        return new BasicSettingProvider();
    }

    public void Initialize(Action onReady)
    {
        onReady?.Invoke();
    }

    public void SetSetting(SettingType settingType, float value)
    {
        var settingData = GameInstaller.Instance.SystemLocator.DataManager
            .GetData<Dictionary<SettingType, float>>(GameDataType.SettingsFloat);
        if (!settingData.TryAdd(settingType, value))
        {
            settingData[settingType] = value;
        }
        GameInstaller.Instance.SystemLocator.DataManager.SetData(GameDataType.SettingsFloat, settingData);
    }
    public void SetSetting(SettingType settingType, string value)
    {
        var settingData = GameInstaller.Instance.SystemLocator.DataManager
            .GetData<Dictionary<SettingType, string>>(GameDataType.SettingsString);
        if (!settingData.TryAdd(settingType, value))
        {
            settingData[settingType] = value;
        }
        GameInstaller.Instance.SystemLocator.DataManager.SetData(GameDataType.SettingsString, settingData);
    }

    public void GetSetting<T>(SettingType settingType, out T value)
    {
        var type = typeof(T);
        value = default;
        if (type==typeof(float))
        {
            var settingData = GameInstaller.Instance.SystemLocator.DataManager
                .GetData<Dictionary<SettingType, float>>(GameDataType.SettingsFloat);
            var tempvalue = (settingData.TryGetValue(settingType, out var valueFloat) ? valueFloat : 1);
            value = (T)Convert.ChangeType(tempvalue, typeof(T));
        }else if (type == typeof(string))
        {
            var settingData = GameInstaller.Instance.SystemLocator.DataManager
                .GetData<Dictionary<SettingType, String>>(GameDataType.SettingsString);
            var tempvalue = settingData.TryGetValue(settingType, out var valueString) ? valueString : "";
            value = (T)Convert.ChangeType(tempvalue, typeof(T));
        }
        
    }
}
