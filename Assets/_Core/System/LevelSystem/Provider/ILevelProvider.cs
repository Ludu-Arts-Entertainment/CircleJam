using System;

public interface ILevelProvider
{
    ILevelProvider CreateSelf();
    ILevelData CurrentLevelData { get;  }
    void Initialize(Action onReady);
    void LoadLevel(int levelIndex);
    void DisposeLevel();
}