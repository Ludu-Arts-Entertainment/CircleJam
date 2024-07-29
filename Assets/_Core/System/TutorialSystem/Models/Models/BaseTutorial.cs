using System;

public abstract class BaseTutorial : ITrackListener
{
    public Tutorial Tutorial;
    private string _trackType;
    protected Action<BaseTutorial, TutorialState> tutorialStateChanged;

    public delegate void TutorialStateChanged(Tutorial tutorial);

    public abstract void OnTrackTriggered(params object[] args);

    protected void InternalCreate(Tutorial tutorial, string trackType)
    {
        _trackType = trackType;
        Tutorial = tutorial;
        if (!Tutorial.isActive) return;

        if (tutorial.state == TutorialState.Waiting)
        {
            TrackingService.Track(_trackType, this);
        }
    }

    protected virtual void OnGoing()
    {
        SetState(TutorialState.Ongoing);
        /*GameInstaller.Instance.SystemLocater.AnalyticsManager.SendEvent(new AnalyticEvents.TutorialStart()
        {
            StepId = Tutorial.type.ToString()
        });*/
    }


    protected virtual void OnCompleted()
    {
        SetState(TutorialState.Completed);
        TrackingService.UnTrack(_trackType, this);
        /*GameInstaller.Instance.SystemLocater.AnalyticsManager.SendEvent(new AnalyticEvents.TutorialCompleted()
        {
            StepId = Tutorial.type.ToString()
        });*/
    }

    protected virtual void SetState(TutorialState state)
    {
        Tutorial.state = state;
        OnProgressChanged();
    }


    protected void OnProgressChanged()
    {
        tutorialStateChanged?.Invoke(this, Tutorial.state);
    }

    protected virtual void CheckProgress()
    {
        OnGoing();
    }

    public abstract BaseTutorial Create(Tutorial tutorial);
}