using System;

public class EarnXCurrencyFromActivationQuest : BaseQuest
{
    public override BaseQuest Create(IQuestGroupController questGroupController, Quest quest)
    {
        var newQuest = new EarnXCurrencyFromActivationQuest();
        newQuest.InternalCreate(questGroupController, quest, TrackType.CurrencyGained);
        return newQuest;
    }

    public override void OnTrackTriggered(params object[] args)
    {
        if ((string)args[0] == Quest.QuestData.CurrencyType)
        {
            Quest.CurrentAmount += Convert.ToInt32(args[1]);
            Quest.CurrentAmount = Quest.CurrentAmount > Quest.QuestData.Amount ? Quest.QuestData.Amount : Quest.CurrentAmount;
            base.OnTrackTriggered(args);
        }
    }

    public override void Dispose()
    {
        TrackingService.UnTrack(TrackType.CurrencyGained, this);
    }
}