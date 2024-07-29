#if !TrackService_Modified
using System;

public record TrackingDataHistory
{
    public string TrackType;
    public int Amount;
    public DateTime DateTime => TimeHelper.GetCurrentDateTime();
}
#endif