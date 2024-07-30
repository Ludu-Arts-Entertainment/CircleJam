using System;

public interface IMovementProvider
{
    public int MovementCount { get; }
    IMovementProvider CreateSelf();
    void Initialize(Action onReady);
}