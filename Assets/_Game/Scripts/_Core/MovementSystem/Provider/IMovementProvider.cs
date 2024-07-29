using System;

public interface IMovementProvider
{
    IMovementProvider CreateSelf();
    void Initialize(Action onReady);
}