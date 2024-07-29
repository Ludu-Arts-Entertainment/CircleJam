using System.Numerics;
using UnityEngine;

[System.Serializable]
public class Tutorial
{
    public bool isActive = true;
    public TutorialType type;
    public BigInteger CurrentAmount;
    public TutorialState state;
    public TutorialData tutorialData;
    public TutorialMetaData tutorialMetaData;
    public bool isPreventScrolling;


    public Tutorial Clone()
    {
        return new Tutorial()
        {
            isActive = this.isActive,
            type = this.type,
            CurrentAmount = this.CurrentAmount,
            state = this.state,
            tutorialData = this.tutorialData,
            isPreventScrolling = this.isPreventScrolling
        };
    }
}

[System.Serializable]
public class TutorialMetaData
{
    public string text;
    public Color textColor;
}