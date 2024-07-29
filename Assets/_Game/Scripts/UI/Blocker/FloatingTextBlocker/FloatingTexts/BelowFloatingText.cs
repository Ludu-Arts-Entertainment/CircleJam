using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BelowFloatingText : FloatingText
{
    [Header("Animation")]
    [SerializeField] private float moveUpTime = 1f;
    [SerializeField] private Ease moveUpEase = Ease.Unset;
    [SerializeField] private float moveUpAmount = 1f;
    [SerializeField] private float moveUpWaitTime = 1f;
    [SerializeField] private Ease moveDownEase = Ease.Unset;
    [SerializeField] private float moveDownTime = 1f;

    private RectTransform parentCanvasTransform;
    private float startHeight;

    public override void StartAnimation()
    {
        parentCanvasTransform ??= transform.parent.parent.GetComponent<RectTransform>();
        startHeight = parentCanvasTransform.position.y - (parentCanvasTransform.sizeDelta.y / 2) - 100f;

        Debug.Log("BelowFloatingText size: " + parentCanvasTransform.sizeDelta.y);

        ResetAnimation();

        var targetY = startHeight + moveUpAmount;
        transform.DOMoveY(targetY, moveUpTime).SetEase(moveUpEase).OnComplete(() =>
        {
            transform.DOMoveY(startHeight, moveDownTime).SetEase(moveDownEase).SetDelay(moveUpWaitTime).OnComplete(() =>
            {
                DestroySelf();
            });
        });
    }

    protected override void ResetAnimation()
    {
        transform.localPosition = Vector3.zero;
        transform.DOMoveY(startHeight, 0f);
    }
}
