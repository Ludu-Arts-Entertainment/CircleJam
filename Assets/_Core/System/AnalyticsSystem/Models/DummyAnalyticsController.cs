using System;
using System.Threading.Tasks;
using UnityEngine;

public class DummyAnalyticsController : IAnalyticsController
{
    public async Task Initialize()
    {
        await Task.Delay(0);
    }

    public void SendEvent(IAnalyticEvent analyticEvent)
    {
        switch (analyticEvent.EventType)
        {
            case AnalyticEventType.Custom:
                SendCustomEvent(analyticEvent);
                break;
            case AnalyticEventType.Progression:
                SendProgressionEvent(analyticEvent);
                break;
            case AnalyticEventType.Ad:
                SendAdEvent(analyticEvent);
                break;
            case AnalyticEventType.Resource:
                SendResourceEvent(analyticEvent);
                break;
            case AnalyticEventType.IAP:
                SendIAPEvent(analyticEvent);
                break;
        }
    }
    private void SendCustomEvent(IAnalyticEvent analyticEvent)
    {
        Debug.Log($"<color=red>{nameof(DummyAnalyticsController)}: {analyticEvent.Name}</color>\n{analyticEvent.GetJson()}");
    }
    private void SendProgressionEvent(IAnalyticEvent analyticEvent)
    {
        Debug.Log($"<color=red>{nameof(DummyAnalyticsController)}: {analyticEvent.Name}</color>\n{analyticEvent.GetJson()}");
    }
    private void SendAdEvent(IAnalyticEvent analyticEvent)
    {
        Debug.Log($"<color=red>{nameof(DummyAnalyticsController)}: {analyticEvent.Name}</color>\n{analyticEvent.GetJson()}");
    }
    private void SendResourceEvent(IAnalyticEvent analyticEvent)
    {
        Debug.Log($"<color=red>{nameof(DummyAnalyticsController)}: {analyticEvent.Name}</color>\n{analyticEvent.GetJson()}");
    }
    private void SendIAPEvent(IAnalyticEvent analyticEvent)
    {
        Debug.Log($"<color=red>{nameof(DummyAnalyticsController)}: {analyticEvent.Name}</color>\n{analyticEvent.GetJson()}");
    }
}