using System;

public class HapticManager : IManager
{
    private IHapticProvider _hapticProvider;
    private SystemLocator _systemLocator;
    public IManager CreateSelf()
    {
        return new HapticManager();
    }

    public void Initialize(GameInstaller gameInstaller, Action onReady)
    {
        _hapticProvider = HapticProviderFactory.Create(gameInstaller.Customizer.HapticProvider);
        _hapticProvider.Initialize(onReady);
        _systemLocator = gameInstaller.SystemLocator;
        _systemLocator.SettingManager.OnSettingChanged += OnSettingChanged;
    }
    public bool IsReady()
    {
        return _hapticProvider != null;
    }
    private void OnSettingChanged(SettingType obj)
    {
        switch (obj)
        {
            case SettingType.Haptic:
                _systemLocator.SettingManager.GetSetting<float>(SettingType.Haptic, out var hapticVolume);
                _hapticProvider.SetHapticState(hapticVolume);
                break;
        }
    }

    public void Success()
    {
        _hapticProvider.Success();
    }
    public void Failure()
    {
        _hapticProvider.Failure();
    }
    public void Heavy()
    {
        _hapticProvider.Heavy();
    }
    public void Light()
    {
        _hapticProvider.Light();
    }
    public void Medium()
    {
        _hapticProvider.Medium();
    }
    public void Warning()
    {
        _hapticProvider.Warning();
    }
    public void CustomHaptic(CustomHapticData customHapticData)
    {
        _hapticProvider.CustomHaptic(customHapticData);
    }
}