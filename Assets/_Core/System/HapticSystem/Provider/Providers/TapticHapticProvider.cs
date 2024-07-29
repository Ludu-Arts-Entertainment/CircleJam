using System;
public class TapticHapticProvider : IHapticProvider
{
    public IHapticProvider CreateSelf()
    {
        return new TapticHapticProvider();
    }

    public void Initialize(Action onReady)
    {
        GameInstaller.Instance.SystemLocator.SettingManager.GetSetting<float>(SettingType.Haptic, out var hapticVolume);
        Taptic.tapticOn = hapticVolume > 0;
        onReady();
    }

    public void SetHapticState(float hapticVolume)
    {
        Taptic.tapticOn = hapticVolume > 0;
    }

    public void Success()
    {
        Taptic.Success();
    }

    public void Failure()
    {
        Taptic.Failure();
    }
    public void Heavy()
    {
        Taptic.Heavy();
    }
    public void Light()
    {
        Taptic.Light();
    }
    public void Medium()
    {
        Taptic.Medium();
    }
    public void Warning()
    {
        Taptic.Warning();
    }

    public void CustomHaptic(CustomHapticData customHapticData)
    {
        Taptic.Selection();
    }
}
