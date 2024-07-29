using UnityEngine;

public class ShowButtonTutorial : BaseTutorial
{
    private object[] _args;
    private RectTransform _buttonTransform;

    public BaseTutorial InternalCreate(Tutorial tutorial)
    {
        var newTutorial = new ShowButtonTutorial();
        newTutorial.InternalCreate(tutorial, TrackType.ShowButtonTutorial);
        return newTutorial;
    }

    public override BaseTutorial Create(Tutorial tutorial)
    {
        InternalCreate(tutorial);
        return this;
    }

    public override void OnTrackTriggered(params object[] args)
    {
        _args = args;
        _buttonTransform = (RectTransform) args[0];
        Tutorial.CurrentAmount = 1;
        CheckProgress();
        OnProgressChanged();
    }

    protected override void OnGoing()
    {
        TrackingService.UnTrack(TrackType.ShowButtonTutorial, this);

        GameInstaller.Instance.SystemLocator.EventManager.Subscribe<Events.TutorialCompleted>(OnTutorialComplete);

        var meta = Tutorial.tutorialMetaData;

        var TapData = new TutorialTapData(_buttonTransform.position, animated:true, description: meta.text);
        var maskData = new MaskData(MaskType.Square, _buttonTransform.position ,_buttonTransform.rect.size);
        TutorialUIData tutorialUIData = new TutorialUIData(tapData:TapData,maskData:maskData);
        GameInstaller.Instance.SystemLocator.UIManager.Show(UITypes.TutorialBlocker, tutorialUIData);
        base.OnGoing();
    }

    private void OnTutorialComplete(Events.TutorialCompleted tutorialCompleted)
    {
        if (tutorialCompleted.TutorialType == TutorialType.ShowButton)
        {
            BaseComplete();
        }
    }

    private void BaseComplete()
    {
        GameInstaller.Instance.SystemLocator.EventManager.Unsubscribe<Events.TutorialCompleted>(OnTutorialComplete);
        GameInstaller.Instance.SystemLocator.UIManager.Hide(UITypes.TutorialBlocker);
        base.OnCompleted();
    }
}
public partial class TrackType
{
    public const string ShowButtonTutorial = "ShowButtonTutorial";
}
