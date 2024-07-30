using System;

public interface IGoalProvider
{
    IGoalProvider CreateSelf();
    void Initialize(Action onReady);
}