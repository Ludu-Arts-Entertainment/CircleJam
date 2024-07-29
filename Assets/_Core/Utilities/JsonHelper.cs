using System;
using UnityEngine;

public static class JsonHelper
{
    public static string ToJson<T>(T value)
    {
        return Newtonsoft.Json.JsonConvert.SerializeObject(value);
    }

    public static T FromJson<T>(string value, Type type = null)
    {
        return (T)Newtonsoft.Json.JsonConvert.DeserializeObject(value, type ?? typeof(T));
    }
    public static object FromJson(string value, Type type = null)
    {
        return Newtonsoft.Json.JsonConvert.DeserializeObject(value, type);
    }
}