public class BasicDailyQuestGroupController : BaseQuestGroupController
{
    public override IQuestGroupController Create()
    {
        return new BasicDailyQuestGroupController();
    }

    public override void QuestClaim(Quest quest)
    {
        base.QuestClaim(quest);
        QuestDispose(quest);
        GiverService.Give(quest.RewardProducts, ActivateNextQuest);
    }
}