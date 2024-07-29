using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MoveAndHideFloatingText : FloatingText
{
    [Header("Animation")]
    [SerializeField] private float animationTime = 2f;
    [SerializeField] private float moveAmount = 200f;
    [SerializeField] private float fadeAmount = 0f;
    [SerializeField] private Ease moveEase = Ease.OutCubic;
    [SerializeField] private Ease fadeEase = Ease.Linear;

    private CanvasGroup CanvasGroup => canvasGroup ? canvasGroup : canvasGroup = GetComponent<CanvasGroup>();
    private CanvasGroup canvasGroup;
    public override void StartAnimation()
    {
        ResetAnimation();

        var targetPosition = transform.position.y + moveAmount;
        transform.localScale = Vector3.one;
        CanvasGroup.DOFade(fadeAmount, animationTime).SetEase(fadeEase);
        transform.DOMoveY(targetPosition, animationTime).SetEase(moveEase).OnComplete(() => 
        {
            DestroySelf();
        });
    }

    protected override void ResetAnimation()
    {
        CanvasGroup.alpha = 1f;
        transform.localPosition = Vector3.zero;
    }
}
