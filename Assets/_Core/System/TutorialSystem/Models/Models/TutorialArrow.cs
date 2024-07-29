using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TutorialArrow : MonoBehaviour
{
    [SerializeField] private GameObject arrowObject;

    private Sequence sequence;

    private void Awake() 
    {
        StartArrowAnimation();
    }

    private void StartArrowAnimation()
    {
        sequence = DOTween.Sequence();
        sequence.Append(arrowObject.transform.DOLocalMoveY(2f, 0.5f).SetEase(Ease.InOutSine));
        sequence.Append(arrowObject.transform.DOLocalMoveY(1.3f, 0.5f).SetEase(Ease.InOutSine));
        sequence.SetLoops(-1, LoopType.Yoyo);
        sequence.SetUpdate(true);
    }
}
