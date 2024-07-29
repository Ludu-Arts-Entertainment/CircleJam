using System;
public class AnalyticsManager : IManager
{
    private IAnalyticsSystemProvider _analyticsSystemProvider;
    public IManager CreateSelf()
    {
        return new AnalyticsManager();
    }

    public void Initialize(GameInstaller gameInstaller, Action onReady)
    {
        _analyticsSystemProvider = AnalyticsProviderFactory.Create(gameInstaller.Customizer.AnalyticsProvider);
        _analyticsSystemProvider.Initialize(onReady);
    }

    public bool IsReady()
    {
        return _analyticsSystemProvider != null;
    }
    public void SendEvent(IAnalyticEvent analyticEvent)
    {
        _analyticsSystemProvider.SendEvent(analyticEvent);
    }
}
