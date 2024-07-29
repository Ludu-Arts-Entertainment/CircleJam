using System.Collections.Generic;

public enum SpecialOfferProviderEnums
{
    BasicSpecialOfferProvider
}
public static class SpecialOfferProviderFactory
{
    private static Dictionary<SpecialOfferProviderEnums,ISpecialOfferProvider> _specialOfferProviderDictionary = new ()
    {
        {SpecialOfferProviderEnums.BasicSpecialOfferProvider, new BasicSpecialOfferProvider()},
    };
    
    public static ISpecialOfferProvider Create(SpecialOfferProviderEnums providerEnum)
    {
        return _specialOfferProviderDictionary.TryGetValue(providerEnum, out var provider) ? provider.CreateSelf() : null;
    }
}
