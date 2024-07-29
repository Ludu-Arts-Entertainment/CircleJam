using System;

public interface IGridProvider 
{
    IGridProvider CreateSelf();
    void Initialize(Action onReady);
}