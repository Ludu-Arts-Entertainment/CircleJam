using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Dictionary<AnimationType, string> animationKeys = new Dictionary<AnimationType, string>
    {
        { AnimationType.Idle, "Idle" },
        { AnimationType.Walk, "Walk" },
        { AnimationType.Run, "Run" },
        { AnimationType.Victory, "Victory"}
    };

    private Animator _animator;
    private Animator Animator => _animator ??= GetComponent<Animator>();

    public void PlayAnimation(AnimationType animationType)
    {
        Animator.Play(animationKeys[animationType]);
    }
}

public enum AnimationType
{
    Idle,
    Walk,
    Run,
    Victory,
}