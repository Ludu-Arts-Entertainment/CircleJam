#if FirebaseAnalytics_Enabled
using System.Threading.Tasks;
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseAnalyticsController : IAnalyticsController
{
    public async Task Initialize()
    {
        await FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                Debug.Log("Firebase correctly Initialized");
            }
            else
            {
                Debug.Log("Could not resolve all Firebase dependencies: " + task.Result);
            }
        });
    }
    public void SendEvent(IAnalyticEvent analyticEvent)
    {
        SendCustomEvent(analyticEvent);
        #if UNITY_EDITOR
        Debug.Log($"<color=blue>(FirebaseAnalyticsController): {analyticEvent.Name}</color>\n{analyticEvent.GetJson()}");
        #endif
    }

    private void SendCustomEvent(IAnalyticEvent analyticEvent)
    {
        FirebaseAnalytics.LogEvent(analyticEvent.Name, analyticEvent.GetParameters());
    }
}
#endif