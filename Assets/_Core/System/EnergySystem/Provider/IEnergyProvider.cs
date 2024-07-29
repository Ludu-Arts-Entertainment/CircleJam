using System;

public interface IEnergyProvider
{
    IEnergyProvider CreateSelf();
    void Initialize(Action onReady);
    void Add(int amount);
    void Use(int amount);
    int Get();
    DateTime GetLastGivenTime();
    long GetRemainingTime();
    (bool, ulong) GetUnlimitedEnergyTime();
    void SetUnlimitedEnergyTime(ulong time);
    event Action<int> OnEnergyChanged;
}