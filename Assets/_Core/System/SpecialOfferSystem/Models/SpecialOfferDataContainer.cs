using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpecialOfferDataContainer", menuName = "ScriptableObjects/SpecialOfferDataContainer")]
public class SpecialOfferDataContainer : ScriptableObject
{
    public List<SpecialOfferData> ListOfSpecialOfferData;
}
[Serializable]
public class SpecialOfferData
{
    public string Id;
    public string Title;
    public string Description;
    public List<ProductBlock> ProductBlocks;
    public Sprite Icon;
    public string ListOfProductBlockId;
    public List<TrackTypeValueTuple> TrackTypeValueTuples;
    public List<RequirementTypeValueTuple> RequirementTypeValueTuples;
    
    public bool RequirementCheck()
    {
        return RequirementService.Check(RequirementTypeValueTuples);
    }
}

[Serializable]
public class TrackTypeValueTuple
{
    public string TrackType;
    public int MinValue;
    public int MaxValue;
    public string LastScreen;
    public string NextScreen;
    
    public TrackTypeValueTuple (string trackType, int minValue, int maxValue, string lastScreen, string nextScreen)
    {
        TrackType = trackType;
        MinValue = minValue;
        MaxValue = maxValue;
        LastScreen = lastScreen;
        NextScreen = nextScreen;
    }
}

