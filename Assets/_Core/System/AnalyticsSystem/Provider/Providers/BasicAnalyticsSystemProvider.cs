using System;
using System.Collections.Generic;

public class BasicAnalyticsSystemProvider : IAnalyticsSystemProvider
{
    private List<IAnalyticsController> _analyticsControllers = new List<IAnalyticsController>()
    {
#if UNITY_EDITOR
        new DummyAnalyticsController(),
#endif
#if FirebaseAnalytics_Enabled
        new FirebaseAnalyticsController(), // if you want to use firebase analytics controller add FirebaseAnalytics_Enabled to Scripting Define Symbols
#endif
#if GameAnalytics_Enabled
        new GameAnalyticsController(), // if you want to use game analytics controller add GameAnalytics_Enabled to Scripting Define Symbols
#endif    
        //new AdjustAnalyticsController(),
        //new FacebookAnalyticsController()
    };

public IAnalyticsSystemProvider CreateSelf()
    {
        return new BasicAnalyticsSystemProvider();
    }

    public async void Initialize(Action onReady)
    {
        foreach (var controller in _analyticsControllers)
        {
            await controller.Initialize();
        }
        onReady?.Invoke();
    }
    public void SendEvent(IAnalyticEvent analyticEvent)
    {
        foreach (var controller in _analyticsControllers)
        {
            controller.SendEvent(analyticEvent);
        }
    }
}
