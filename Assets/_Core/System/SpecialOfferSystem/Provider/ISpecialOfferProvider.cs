using System;
using System.Collections.Generic;

public interface ISpecialOfferProvider
{
    ISpecialOfferProvider CreateSelf();
    void Initialize(Action onReady);
    void Trigger(SpecialOfferData specialOfferData);
    List<SpecialOfferData> FilteredRequirementsCheck();
}
