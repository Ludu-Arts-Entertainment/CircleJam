using System.Threading.Tasks;

public interface IAnalyticsController
{
    Task Initialize();
    void SendEvent(IAnalyticEvent analyticEvent);
}
