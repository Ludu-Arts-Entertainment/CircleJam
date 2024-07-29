using System;

public class SpecialOfferTrackListener : ITrackListener
{
    public void Initialize()
    {
        TrackingService.Track(TrackType.LevelUp,this,LevelUpFilter);
        TrackingService.Track(TrackType.AdWatched,this,AdWatchedFilter);
        TrackingService.Track(TrackType.ScreenChanged,this,ScreenChangedFilter);
    }

    public void Dispose()
    {
        TrackingService.UnTrack(TrackType.LevelUp,this);
        TrackingService.UnTrack(TrackType.AdWatched,this);
        TrackingService.UnTrack(TrackType.ScreenChanged,this);
    }
    SpecialOfferData _specialOfferData;
    public void OnTrackTriggered(params object[] args)
    {
        GameInstaller.Instance.SystemLocator.SpecialOfferManager.Trigger(_specialOfferData);
    }
    private bool LevelUpFilter(object[] args)
    {
        var list = GameInstaller.Instance.SystemLocator.SpecialOfferManager.FilteredRequirementsCheck();
        var level = (int) args[0];
        foreach (var sod in list)
        {
            foreach (var sodTrackTypeValueTuple in sod.TrackTypeValueTuples)
            {
                if (sodTrackTypeValueTuple.TrackType==TrackType.LevelUp && sodTrackTypeValueTuple.MinValue<=level && sodTrackTypeValueTuple.MaxValue>=level)
                {
                    _specialOfferData = sod;
                    return true;
                }
            }
        }
        return false;
    }
    private bool AdWatchedFilter(object[] args)
    {
        var list = GameInstaller.Instance.SystemLocator.SpecialOfferManager.FilteredRequirementsCheck();
        var count = 5;//TODO: Get watched ad count
            
        foreach (var sod in list)
        {
            foreach (var sodTrackTypeValueTuple in sod.TrackTypeValueTuples)
            {
                if (sodTrackTypeValueTuple.TrackType==TrackType.AdWatched && sodTrackTypeValueTuple.MinValue<=count && sodTrackTypeValueTuple.MaxValue>=count)
                {
                    _specialOfferData = sod;
                    return true;
                }
            }
        }
        return false;
    }
    private bool ScreenChangedFilter(object[] args)
    {
        var list = GameInstaller.Instance.SystemLocator.SpecialOfferManager.FilteredRequirementsCheck();
        (string,string) screenChanged = ("","");
        try
        {
            var lastScreen = (string) args[0];
            var nextScreen = (string) args[1];
            screenChanged = (lastScreen,nextScreen);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
            
        foreach (var sod in list)
        {
            foreach (var sodTrackTypeValueTuple in sod.TrackTypeValueTuples)
            {
                if (sodTrackTypeValueTuple.TrackType==TrackType.ScreenChanged 
                    && (sodTrackTypeValueTuple.LastScreen == screenChanged.Item1 || string.IsNullOrEmpty(sodTrackTypeValueTuple.LastScreen))
                    && (sodTrackTypeValueTuple.NextScreen == screenChanged.Item2 || string.IsNullOrEmpty(sodTrackTypeValueTuple.NextScreen)))
                {
                    _specialOfferData = sod;
                    return true;
                }
            }
        }
        return false;
    }
}
