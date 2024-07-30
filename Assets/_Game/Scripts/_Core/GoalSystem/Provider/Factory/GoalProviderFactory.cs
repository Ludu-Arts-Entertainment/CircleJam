using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GoalProviderEnums
{
    CircleJamGoalProvider,
}

public static class GoalProviderFactory
{
    private static Dictionary<GoalProviderEnums, IGoalProvider> _goalProviderDictionary = new ()
    {
        {GoalProviderEnums.CircleJamGoalProvider, new CircleJamGoalProvider()}
    };
    
    public static IGoalProvider Create(GoalProviderEnums providerEnum)
    {
        return _goalProviderDictionary.TryGetValue(providerEnum, out var provider) ? provider.CreateSelf() : null;
    }
}