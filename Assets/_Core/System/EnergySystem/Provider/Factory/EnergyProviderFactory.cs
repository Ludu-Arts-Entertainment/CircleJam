using System.Collections.Generic;

public enum EnergyProviderEnums
{
    BasicEnergyProvider,
}
public static class EnergyProviderFactory
{
    private static Dictionary<EnergyProviderEnums,IEnergyProvider> _energyProviderDictionary = new ()
    {
        {EnergyProviderEnums.BasicEnergyProvider, new BasicEnergyProvider()},
    };
    
    public static IEnergyProvider Create(EnergyProviderEnums providerEnum)
    {
        return _energyProviderDictionary.TryGetValue(providerEnum, out var provider) ? provider.CreateSelf() : null;
    }
}