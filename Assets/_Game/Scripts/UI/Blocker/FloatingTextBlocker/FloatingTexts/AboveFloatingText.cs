using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AboveFloatingText : FloatingText
{
   [Header("Animation")]
    [SerializeField] private float moveDownTime = 1f;
    [SerializeField] private Ease moveDownEase = Ease.Unset;
    [SerializeField] private float moveDownAmount = 1f;
    [SerializeField] private float moveDownWaitTime = 1f;
    [SerializeField] private Ease moveUpEase = Ease.Unset;
    [SerializeField] private float moveUpTime = 1f;

    private RectTransform parentCanvasTransform;
    private float startHeight;

    public override void StartAnimation()
    {
        parentCanvasTransform ??= transform.parent.parent.GetComponent<RectTransform>();

        Debug.Log("AboveFloatingText size: " + parentCanvasTransform.sizeDelta.y);
        startHeight = parentCanvasTransform.sizeDelta.y + 100f;

        ResetAnimation();
        
        var targetY = startHeight - moveDownAmount;

        transform.DOMoveY(targetY, moveDownTime).SetEase(moveDownEase).OnComplete(() =>
        {
            transform.DOMoveY(startHeight, moveUpTime).SetEase(moveUpEase).SetDelay(moveDownWaitTime).OnComplete(() =>
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
