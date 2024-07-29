using System;

public interface IManager
{
    IManager CreateSelf();
    void Initialize(GameInstaller gameInstaller, Action onReady);
    bool IsReady();
}
