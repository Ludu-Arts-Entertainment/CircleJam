using System;
using System.Collections.Generic;
using UnityEngine;

public static class TutorialFactory
{
    private static readonly Dictionary<TutorialType, BaseTutorial> Tutorial =
        new Dictionary<TutorialType, BaseTutorial>()
        {
            //{TutorialType.ShowButton, new ShowButtonTutorial()},
        };

    public static BaseTutorial Create(Tutorial tutorial)
    {
        return Tutorial.TryGetValue(tutorial.type, out var tTutorial) == false ? null : tTutorial.Create(tutorial);
    }
}