using System;

public class GUIDHelper
{
    public static string GetUniqueGUID()
    {
        return GetGuid() + $"-{DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds}";
    }
    public static string GetGuid()
    {
        return Guid.NewGuid().ToString();
    }
}
