using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDataContainer", menuName = "ScriptableObjects/Service/CharacterDataContainer", order = 0)]
public class CharacterDataContainer : ScriptableObject
{
    public List<CharacterData> Characters = new List<CharacterData>();

    public CharacterData GetCharacterByColor(GoalColor color)
    {
        foreach (var character in Characters)
        {
            if (character.Color == color)
            {
                return character;
            }
        }

        return null;
    }
}

[Serializable]
public class CharacterData
{
    public GoalColor Color;
    public Material Material;
}
