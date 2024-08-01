using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDataService 
{
    private static bool _isReady;
    private static CharacterDataContainer _characterDataContainer;
    private static void Initialize()
    {
        _characterDataContainer = Resources.Load<CharacterDataContainer>("CharacterDataContainer");
        _isReady = true;
    }
    public static CharacterData GetCharacterByColor(GoalColor color)
    {
        if (_isReady) return _characterDataContainer.GetCharacterByColor(color);
        Initialize();
        return _characterDataContainer.GetCharacterByColor(color);
    }
}