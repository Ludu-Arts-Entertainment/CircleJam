public class SpendXCurrencyFromStartQuest : BaseQuest
{
    public override BaseQuest Create(IQuestGroupController questGroupController, Quest quest)
    {
        var newQuest = new SpendXCurrencyFromStartQuest();
        newQuest.InternalCreate(questGroupController, quest, TrackType.CurrencySpend);
        return newQuest;
    }

    public override void OnTrackTriggered(params object[] args)
    {
        if ((string)args[0] == Quest.QuestData.CurrencyType)
        {
            Quest.CurrentAmount =
                GameInstaller.Instance.SystemLocator.ExchangeManager.GetExchange(Quest.QuestData.CurrencyType, 0f);
            base.OnTrackTriggered(args);
        }
    }


    public override void Dispose()
    {
        TrackingService.UnTrack(TrackType.CurrencySpend, this);
    }

}