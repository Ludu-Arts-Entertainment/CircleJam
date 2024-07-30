using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private AnimationController _animationController;
    [SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;

    public void Initialize(GoalColors color)
    {
        var characterData = CharacterDataService.GetCharacterByColor(color);
        _skinnedMeshRenderer.material = characterData.Material;

        _animationController.PlayAnimation(AnimationType.Idle);
    }
}