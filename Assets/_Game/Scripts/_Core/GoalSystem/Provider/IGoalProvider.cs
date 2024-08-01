using System;
using System.Collections.Generic;

public interface IGoalProvider
{
    IGoalProvider CreateSelf();
    void Initialize(Action onReady);
    void Reset();
    void UpdateLeveledGoal();
    int CurrentGoalCount { get; }
    List<GoalColor> LeveledGoalColors { get; }
}