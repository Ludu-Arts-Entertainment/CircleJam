using System;

public class SettingManager : IManager
{
    public event Action<SettingType> OnSettingChanged;
    
    private ISettingProvider _settingProvider;
    public IManager CreateSelf()
    {
        return new SettingManager();
    }

    public void Initialize(GameInstaller gameInstaller, Action onReady)
    {
        _settingProvider = SettingProviderFactory.Create(gameInstaller.Customizer.SettingProvider);
        _settingProvider.Initialize(onReady);
    }

    public bool IsReady()
    {
        return _settingProvider != null;
    }
    public void SetSetting(SettingType settingType, float value)
    {
        _settingProvider.SetSetting(settingType, value);
        OnSettingChanged?.Invoke(settingType);
    }
    public void SetSetting(SettingType settingType, string value)
    {
        _settingProvider.SetSetting(settingType, value);
        OnSettingChanged?.Invoke(settingType);
    }

    public void GetSetting<T>(SettingType settingType, out T value)
    {
        _settingProvider.GetSetting(settingType, out value);
    }
}

public enum SettingType
{
    Sound,
    Haptic,
    Music
}
