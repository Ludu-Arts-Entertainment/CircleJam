using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SystemLocator
{
    private DailyOfferManager _dailyOfferManager;
    public DailyOfferManager DailyOfferManager =>
        _dailyOfferManager ??= GameInstaller.Instance.ManagerDictionary[ManagerEnums.DailyOfferManager] as DailyOfferManager;
}