using System;

public class TimeHelper
{
    public static DateTime GetCurrentDateTime()
    {
        return DateTime.UtcNow;
    }
    public static DateTime UnixTimeStampToDateTime(ulong unixTimeStamp)
    {
        return new DateTime(1970, 1, 1).AddSeconds(unixTimeStamp);
    }
    public static ulong DateTimeToUnixTimeStampInSeconds(DateTime dateTime)
    {
        return (ulong)(dateTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
    }
    public static ulong DateTimeToUnixTimeStampInMilliSeconds(DateTime dateTime)
    {
        return (ulong)(dateTime.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
    }
}