public abstract class BaseQuest : ITrackListener
{
    public Quest Quest;
    protected IQuestGroupController QuestGroupController;
    public abstract BaseQuest Create(IQuestGroupController questGroupController, Quest quest);

    protected void InternalCreate(IQuestGroupController questGroupController, Quest quest, string trackType)
    {
        QuestGroupController = questGroupController;
        Quest = quest;
        if (quest.State != QuestState.Completed)
        {
            TrackingService.Track(trackType, this);
        }
    }
    public virtual void OnTrackTriggered(params object[] args)
    {
        GameInstaller.Instance.SystemLocator.QuestManager.QuestProgressChange(Quest,QuestGroupController.QuestGroup.Id);
    }
    public abstract void Dispose();
}