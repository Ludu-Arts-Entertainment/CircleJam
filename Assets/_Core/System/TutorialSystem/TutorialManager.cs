using System;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : IManager
{
    private ITutorialProvider _tutorialProvider;
    public List<Tutorial> _tutorials = new List<Tutorial>();
    private bool _isLoaded = false;

    private Dictionary<TutorialType, TutorialState> _tutorialStates =
        new Dictionary<TutorialType, TutorialState>();


    public IManager CreateSelf()
    {
        return new TutorialManager();
    }

    public void Initialize(GameInstaller gameInstaller, Action onReady)
    {
        _tutorialProvider = TutorialProviderFactory.Create(gameInstaller.Customizer.TutorialProvider);
        _tutorialProvider.Initialize(onReady);
        Load();
    }

    public bool IsReady()
    {
        return _tutorialProvider != null;
    }

    public void Load()
    {
        if (_isLoaded) return;
        _isLoaded = true;
        var tutorialContainer = Resources.Load<TutorialContainer>("TutorialContainer");
        _tutorials = tutorialContainer?.tutorials;
        _tutorials??= new List<Tutorial>();
        Debug.Log("Loaded Tutorial");
        _tutorialStates =
            GameInstaller.Instance.SystemLocator.DataManager.GetData<Dictionary<TutorialType, TutorialState>>(
                GameDataType.TutorialData);
        SetStates();
        LoadTutorials();
    }

    private void SetStates()
    {
        foreach (var t in _tutorials)
        {
            t.state = _tutorialStates[t.type];
        }
    }

    private void LoadTutorials()
    {
        if (!_isLoaded) Load();
        foreach (var tutorial in _tutorials)
        {
            var newTutorial = TutorialFactory.Create(tutorial);
            if (newTutorial == null) continue;
        }
    }

    public TutorialState GetTutorialState(TutorialType type)
    {
        foreach (var t in _tutorials)
        {
            if (t.type == type)
            {
                return t.state;
            }
        }

        return TutorialState.Completed;
    }

    public bool IsAnyPreventScrolling()
    {
        foreach (var t in _tutorials)
        {
            if (t.state == TutorialState.Ongoing && t.isPreventScrolling)
            {
                return true;
            }
        }

        return false;
    }

    public Tutorial GetActiveTutorial()
    {
        foreach (var t in _tutorials)
        {
            if (t.state == TutorialState.Ongoing)
            {
                return t;
            }
        }

        return null;
    }

    public bool IsTutorialPlaying(TutorialType type)
    {
        foreach (var t in _tutorials)
        {
            if (t.type == type && t.state == TutorialState.Ongoing)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsTutorialCompleted(TutorialType type)
    {
        foreach (var t in _tutorials)
        {
            if (t.type == type && t.state == TutorialState.Completed)
            {
                return true;
            }
        }

        return false;
    }

    public int GetTutorialId(TutorialType type)
    {
        foreach (var t in _tutorials)
        {
            if (t.type == type)
            {
                return t.tutorialData.id;
            }
        }

        return -1;
    }
}

public partial class Events
{
    public class TutorialCompleted : IEvent
    {
        public TutorialType TutorialType;
        public TutorialCompleted(TutorialType tutorialType)
        {
            TutorialType = tutorialType;
        }
    }
}