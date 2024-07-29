public class EarnXCurrencyFromStartQuest : BaseQuest
{
    public override BaseQuest Create(IQuestGroupController questGroupController, Quest quest)
    {
        var newQuest = new EarnXCurrencyFromStartQuest();
        newQuest.InternalCreate(questGroupController, quest, TrackType.CurrencyGained);
        return newQuest;
    }

    public override void OnTrackTriggered(params object[] args)
    {
        if ((string)args[0] == Quest.QuestData.CurrencyType)
        {
            Quest.CurrentAmount = GameInstaller.Instance.SystemLocator.ExchangeManager.GetExchange(Quest.QuestData.CurrencyType,0f);
            base.OnTrackTriggered(args);
        }
    }


    public override void Dispose()
    {
        TrackingService.UnTrack(TrackType.CurrencyGained, this);
    }
}