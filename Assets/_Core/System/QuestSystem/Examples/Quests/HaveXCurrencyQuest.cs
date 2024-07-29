using System;
using System.Numerics;
using UnityEngine;

public class HaveXCurrencyQuest : BaseQuest
{
    public override BaseQuest Create(IQuestGroupController questGroupController, Quest quest)
    {
        var newQuest = new HaveXCurrencyQuest();
        newQuest.InternalCreate(questGroupController, quest, TrackType.CurrencyGained);
        return newQuest;
    }

    public override void OnTrackTriggered(params object[] args)
    {
        if ((string)args[0] == Quest.QuestData.CurrencyType)
        {
            Quest.CurrentAmount =
                GameInstaller.Instance.SystemLocator.ExchangeManager.GetExchange(Quest.QuestData.CurrencyType, 0f);
            Quest.CurrentAmount = Quest.CurrentAmount > Quest.QuestData.Amount
                ? Quest.QuestData.Amount
                : Quest.CurrentAmount;
            base.OnTrackTriggered(args);
        }
    }

    public override void Dispose()
    {
        TrackingService.UnTrack(TrackType.CurrencyGained, this);
    }
}