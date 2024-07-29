using System;
using System.Collections.Generic;

public class SpecialOfferManager : IManager
{
    private ISpecialOfferProvider _specialOfferProvider;
    public IManager CreateSelf()
    {
        return new SpecialOfferManager();
    }

    public void Initialize(GameInstaller gameInstaller, Action onReady)
    {
        _specialOfferProvider = SpecialOfferProviderFactory.Create(gameInstaller.Customizer.SpecialOfferProviderEnums);
        _specialOfferProvider.Initialize(onReady);
    }

    public bool IsReady()
    {
        return _specialOfferProvider != null;
    }

    public void Trigger(SpecialOfferData specialOfferData)
    {
        _specialOfferProvider.Trigger(specialOfferData);
    }
    public List<SpecialOfferData> FilteredRequirementsCheck()
    {
        return _specialOfferProvider.FilteredRequirementsCheck();
    }
}
