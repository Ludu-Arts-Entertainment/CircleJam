using System.Collections.Generic;
using Newtonsoft.Json;
#if FirebaseAnalytics_Enabled
using Firebase.Analytics;
#endif

public interface IAnalyticEvent
{
    [JsonIgnore] AnalyticEventType EventType { get; }
    [JsonIgnore] string Name { get; }
    [JsonIgnore] AnalyticsProgressionStatus ProgressionStatus => AnalyticsProgressionStatus.Undefined;
    string GetJson();
#if FirebaseAnalytics_Enabled
    Parameter[] GetParameters();
#endif
    Dictionary<string, object> GetDictionary();
}

//GameAnalyticsILRD.SubscribeMaxImpressions();