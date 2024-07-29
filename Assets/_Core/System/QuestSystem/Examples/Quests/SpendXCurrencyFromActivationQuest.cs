using System;
using UnityEngine;

public class SpendXCurrencyFromActivationQuest : BaseQuest
{
    public override BaseQuest Create(IQuestGroupController questGroupController, Quest quest)
    {
        var newQuest = new SpendXCurrencyFromActivationQuest();
        newQuest.InternalCreate(questGroupController, quest, TrackType.CurrencySpend);
        return newQuest;
    }

    public override void OnTrackTriggered(params object[] args)
    {
        if ((string)args[0] == Quest.QuestData.CurrencyType)
        {
            Quest.CurrentAmount += Convert.ToInt32(args[1]);
            base.OnTrackTriggered(args);
        }
    }


    public override void Dispose()
    {
        TrackingService.UnTrack(TrackType.CurrencySpend, this);
    }
}