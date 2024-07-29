using System;
using System.Collections.Generic;
using UnityEngine;

public static class TrackingService
{
    private static readonly Dictionary<String, List<(ITrackListener,Func<object[],bool>)>> Trackers =
        new Dictionary<String, List<(ITrackListener,Func<object[],bool>)>>();

    public static void Feed(String trackType, params object[] args)
    {
        if (Trackers.TryGetValue(trackType, out var trackers) == false)
        {
            Debug.LogWarning("TrackType not found in trackers" + trackType + " From Feed");
            return;
        }

        for (int i = trackers.Count - 1; i >= 0; i--)
        {
            if (trackers[i].Item2(args))trackers[i].Item1.OnTrackTriggered(args);
        }
    }
    
    public static void Track(String trackType, ITrackListener tracker, Func<object[],bool> filter = null)
    {
        if (Trackers.TryGetValue(trackType, out var trackers) == false)
        {
            trackers = new List<(ITrackListener,Func<object[],bool>)>();
            Trackers.Add(trackType, trackers);
        }

        trackers.Add((tracker,filter ?? (a=>true)));
    }

    public static void UnTrack(String trackType, ITrackListener tracker)
    {
        if (Trackers.TryGetValue(trackType, out var trackers) == false)
        {
            Debug.LogWarning("TrackType not found in trackers" + trackType + " From UnTrack");
            return;
        }

        var index = trackers.FindIndex(x => x.Item1 == tracker);
        if(index>=0)trackers.RemoveAt(index);
    }
}