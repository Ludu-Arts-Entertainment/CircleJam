using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HideOutFloatingText : FloatingText
{
    [Header("Animation")]
    [SerializeField] private float startScale = 0f;

    [Header("ScaleUp")]
    [SerializeField] private float scaleUpTime = 0.5f;
    [SerializeField] private float scaleUpWaitTime = 1f;
    [SerializeField] private Ease scaleUpEase = Ease.OutQuint;

    [Header("HideOut")]
    [SerializeField] private float hideOutAmount = 0f;
    [SerializeField] private float hideOutTime = 1f;
    [SerializeField] private Ease hideOutEase = Ease.Linear;

    private CanvasGroup CanvasGroup => canvasGroup ? canvasGroup : canvasGroup = GetComponent<CanvasGroup>();
    private CanvasGroup canvasGroup;
    public override void StartAnimation()
    {
        ResetAnimation();

        transform.DOScale(1, scaleUpTime).SetEase(scaleUpEase).OnComplete(()=>
        {
            CanvasGroup.DOFade(hideOutAmount, hideOutTime).SetEase(hideOutEase).SetDelay(scaleUpWaitTime).OnComplete(()=>
            {
                DestroySelf();
            });
        });
    }

    protected override void ResetAnimation()
    {
        CanvasGroup.alpha = 1f;
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one * startScale;
    }
}
