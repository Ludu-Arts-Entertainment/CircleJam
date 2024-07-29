using System;
using System.Collections.Generic;
using System.Numerics;

public partial class GameData
{
    public Dictionary<TutorialType,TutorialState> TutorialData = new ()
    {
        { TutorialType.ShowButton, TutorialState.Waiting },
    };
}
