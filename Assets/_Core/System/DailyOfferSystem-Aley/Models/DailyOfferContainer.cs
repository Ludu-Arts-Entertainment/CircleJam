using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "DailyOfferContainer", menuName = "ScriptableObjects/DailyOfferContainer", order = 1)]
public class DailyOfferContainer : ScriptableObject
{
    public List<DailyOfferData> DailyOfferDatas = new List<DailyOfferData>();
    private Dictionary<int, DailyOfferData> dailyOfferDataById;
    public Dictionary<int, DailyOfferData> DailyOfferDataById
    {
        get
        {
            if (dailyOfferDataById == null)
            {
                dailyOfferDataById = new Dictionary<int, DailyOfferData>();
                foreach (var dailyOfferData in DailyOfferDatas)
                {
                    dailyOfferDataById.TryAdd(dailyOfferData.Id, dailyOfferData);
                }
            }

            return dailyOfferDataById;
        }
    }
}