public class BasicQuestGroupController : BaseQuestGroupController
{
    public override IQuestGroupController Create()
    {
        return new BasicQuestGroupController();
    }

    public override void QuestClaim(Quest quest)
    {
        base.QuestClaim(quest);
        ActivateNextQuest();
    }
}
