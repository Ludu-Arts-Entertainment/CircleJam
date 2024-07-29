using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TutorialProviderEnums
{
    BasicTutorialProvider,
}
public static class TutorialProviderFactory
{
    // Start is called before the first frame update
    private static Dictionary<TutorialProviderEnums,ITutorialProvider> _tutorialProviderDictionary = new ()
    {
        {TutorialProviderEnums.BasicTutorialProvider, new BasicTutorialProvider()},
    };
    
    public static ITutorialProvider Create(TutorialProviderEnums providerEnum)
    {
        return _tutorialProviderDictionary.TryGetValue(providerEnum, out var provider) ? provider.CreateSelf() : null;
    }
}
