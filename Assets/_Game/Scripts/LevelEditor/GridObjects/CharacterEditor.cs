using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEditor : GridObjectEditor
{
    [SerializeField] protected SkinnedMeshRenderer _renderer;
    public GoalColor characterColor;

   public void SetColor()
    {
        var characterData = CharacterDataService.GetCharacterByColor(characterColor);
        _renderer.material = characterData.Material;
    }
}
