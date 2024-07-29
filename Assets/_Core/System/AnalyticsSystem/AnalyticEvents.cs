#if FirebaseAnalytics_Enabled
using Firebase.Analytics;
#endif
using System.Collections.Generic;
using Newtonsoft.Json;

public partial class AnalyticEvents
{
    public struct BasicEvent : IAnalyticEvent
    {
        public int level;
        [JsonIgnore] public AnalyticEventType EventType => AnalyticEventType.Custom;
        [JsonIgnore] public string Name => "BasicEvent";

        public string GetJson()
        {
            return JsonHelper.ToJson(this);
        }

        public Dictionary<string, object> GetDictionary()
        {
            return new Dictionary<string, object>()
            {
                { "level", level }
            };
        }
#if FirebaseAnalytics_Enabled
        public Parameter[] GetParameters()
        {
            return new []
            {
                new Parameter("level", level)
            };
        }
#endif
    }
}