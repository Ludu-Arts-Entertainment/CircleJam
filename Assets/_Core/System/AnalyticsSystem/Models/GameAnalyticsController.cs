#if GameAnalytics_Enabled
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GameAnalyticsSDK;
using UnityEngine;
using Utils;

public class GameAnalyticsController : IAnalyticsController
{
    private Dictionary<AnalyticsProgressionStatus, GAProgressionStatus> _statusMapper =
                                                                                                                                                                                                                                                                                                                                 new Dictionary<AnalyticsProgressionStatus, GAProgressionStatus>()
    {
        {AnalyticsProgressionStatus.Start, GAProgressionStatus.Start},
        {AnalyticsProgressionStatus.Complete, GAProgressionStatus.Complete},
        {AnalyticsProgressionStatus.Fail, GAProgressionStatus.Fail},
        {AnalyticsProgressionStatus.Undefined, GAProgressionStatus.Undefined}
    };
    public async Task Initialize()
    {
        GameAnalytics.Initialize();
        #if MaxSdk_Enabled
            GameAnalyticsILRD.SubscribeMaxImpressions();
        #endif
        await AsyncUtils.WaitUntilAsync(new CancellationToken(), () => GameAnalytics.Initialized);
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
                SendCustomEvent(analyticEvent);
                break;
            case AnalyticEventType.Resource:
                SendCustomEvent(analyticEvent);
                break;
            case AnalyticEventType.IAP:
                SendCustomEvent(analyticEvent);
                break;
        }
        #if UNITY_EDITOR

        Debug.Log($"<color=yellow>(GameAnalyticsController): {analyticEvent.Name}</color>\n{analyticEvent.GetJson()}");
        #endif
    }
    private void SendCustomEvent(IAnalyticEvent analyticEvent)
    {
        GameAnalytics.NewDesignEvent(analyticEvent.Name, analyticEvent.GetDictionary());
    }
    private void SendProgressionEvent(IAnalyticEvent analyticEvent)
    {
        GameAnalytics.NewProgressionEvent(_statusMapper[analyticEvent.ProgressionStatus], analyticEvent.Name, analyticEvent.GetDictionary());
    }
}
#endif