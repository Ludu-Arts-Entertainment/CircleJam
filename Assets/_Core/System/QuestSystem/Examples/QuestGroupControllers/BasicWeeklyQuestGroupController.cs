public class BasicWeeklyQuestGroupController : BaseQuestGroupController
{
    public override IQuestGroupController Create()
    {
        return new BasicWeeklyQuestGroupController();
    }

    public override void QuestClaim(Quest quest)
    {
        base.QuestClaim(quest);
        QuestDispose(quest);
        GiverService.Give(quest.RewardProducts, ActivateNextQuest);
    }
}