using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class GoalManager : IManager
{
    public int CurrentGoalCount => _goalProvider.CurrentGoalCount; 
    private IGoalProvider _goalProvider;
    public IManager CreateSelf()
    {
        return new GoalManager();
    }

    public void Initialize(GameInstaller gameInstaller, Action onReady)
    {
        _goalProvider = GoalProviderFactory.Create(gameInstaller.Customizer.GoalProvider);
        _goalProvider.Initialize(onReady);
    }

    public bool IsReady()
    {
        return _goalProvider != null;
    }

    public void Reset()
    {
        _goalProvider.Reset();
    }
}