using System;
using Cysharp.Threading.Tasks;

public class EnergyManager : IManager
{
    private IEnergyProvider _energyProvider;
    public IManager CreateSelf()
    {
        return new EnergyManager();
    }
    public event Action<int> OnEnergyChanged
    {
        add => _energyProvider.OnEnergyChanged += value;
        remove => _energyProvider.OnEnergyChanged -= value;
    }

    public async void Initialize(GameInstaller gameInstaller, Action onReady)
    {
#if RemoteConfigManager_Enabled
        await UniTask.WaitUntil(()=>GameInstaller.Instance.ManagerDictionary.ContainsKey(ManagerEnums.RemoteConfigManager));
#endif
        _energyProvider = EnergyProviderFactory.Create(gameInstaller.Customizer.EnergyProvider);
        _energyProvider.Initialize(onReady);
    }
    public bool IsReady()
    {
        return _energyProvider != null;
    }
    public void Add(int amount)
    {
        _energyProvider.Add(amount);
    }

    public void Use(int amount)
    {
        _energyProvider.Use(amount);
    }
    public DateTime GetLastTime()
    {
        return _energyProvider.GetLastGivenTime();
    }
    public int Get()
    {
        return _energyProvider.Get();
    }
    public long GetRemainingTime()
    {
        return _energyProvider.GetRemainingTime();
    }
    public void SetUnlimitedEnergyTime(ulong time)
    {
        _energyProvider.SetUnlimitedEnergyTime(time);
    }
    public (bool, ulong) GetUnlimitedEnergyTime()
    {
        return _energyProvider.GetUnlimitedEnergyTime();
    }
}