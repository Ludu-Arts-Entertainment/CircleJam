using System;

public interface IAnalyticsSystemProvider
{
    IAnalyticsSystemProvider CreateSelf();
    void Initialize(Action onReady);
    void SendEvent(IAnalyticEvent analyticEvent);
}
