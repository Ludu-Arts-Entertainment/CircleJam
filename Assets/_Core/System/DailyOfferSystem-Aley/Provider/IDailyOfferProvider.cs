using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDailyOfferProvider
{
    IDailyOfferProvider CreateSelf();
    void Initialize(System.Action onReady);
    public Dictionary<int, DailyOfferSaveData> CreateNewDailyOffers();
}
