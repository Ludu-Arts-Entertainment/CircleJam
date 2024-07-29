using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUIData : IBaseUIData
{
    public TutorialTapData tapData;
    public TutorialSlideData slideData;
    public MaskData maskData;
    public HideData hideData;

    public TutorialUIData(TutorialTapData tapData=null,TutorialSlideData slideData=null,MaskData maskData=null, HideData hideData=null)
    {
        this.tapData = tapData;
        this.slideData = slideData;
        this.maskData = maskData;
        this.hideData = hideData;
    }
}


public class TutorialTapData 
{
    public Vector3 posStart;
    public string description;
    public bool animated;
    public TutorialTapData(Vector3 posStart,bool animated=false,string description="")
    {
        this.posStart = posStart;
        this.animated = animated;
        this.description = description;
 
    }
}
public class TutorialSlideData
{
    public Vector3 posStart;
    public Vector3 posEnd;
    public bool animated;
    public string description;
    public TutorialSlideData(Vector3 posStart,Vector3 posEnd,bool animated=false,string description="")
    {
        this.posStart = posStart;
        this.posEnd = posEnd;
        this.animated = animated;
        this.description = description;
    
    }
}

public class MaskData
{
    public MaskType type;
    public Vector3 pos;
    public Vector3 size;


    public MaskData(MaskType type,Vector3 pos,Vector3 size)
    {
        this.type = type;
        this.pos = pos;
        this.size = size;

    }
}

public class HideData
{
    public HideData()
    {
    }
}

public enum MaskType
{
    Circle,
    Square,
}
