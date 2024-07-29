using System;

public interface IHapticProvider
{
    IHapticProvider CreateSelf();
    void Initialize(Action onReady);
    void SetHapticState(float hapticVolume);
    void Success();
    void Failure();
    void Heavy();
    void Light();
    void Medium();
    void Warning();
    void CustomHaptic(CustomHapticData customHapticData);
}
