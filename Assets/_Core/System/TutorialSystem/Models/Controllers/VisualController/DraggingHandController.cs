using System;
using DG.Tweening;
using UnityEngine;

public class DraggingHandController
{
    private const string PoolId = "DraggingHand";
    private static GameObject _draggingHand;
    private static bool _isLoaded;
    private static Action _onComplete;

    private void Awake()
    {
        if (!_isLoaded) LoadHand();
    }

    private static void LoadHand()
    {
        _isLoaded = true;
        //_draggingHand = PoolingSystem.Instance.InstantiateUI(PoolId, Vector2.zero, default);
        _draggingHand.gameObject.SetActive(false);
    }

    public static void Show(Transform t1, Transform t2, Action callback = null)
    {
        var t = Camera.main.WorldToScreenPoint(t1.position);
        if (!_isLoaded) LoadHand();
        t.y = t.y + 90;
        _onComplete = callback;
        _draggingHand.SetActive(true);
        _draggingHand.transform.position = t;
        _draggingHand.transform.localScale = Vector3.one;
        StartHandAnimation(t2);
    }

    public static void Hide()
    {
        _draggingHand.SetActive(false);
    }


    private static void StartHandAnimation(Transform t2)
    {
        // var t = Camera.main.WorldToScreenPoint(t2.position);
        // t.y = t.y + 90;
        // var draggingHand = _draggingHand.GetComponent<DraggingHand>();
        // var t1 = _draggingHand.transform.position;
        // Sequence sequence = DOTween.Sequence();
        // sequence
        //     .Append(_draggingHand.transform.DOMove(t, 2f))
        //     .Append(
        //         _draggingHand.transform.DOScale(Vector3.one, .5f).OnComplete(() => { draggingHand.CloseHand(); }))
        //     .Append(_draggingHand.transform.DOMove(t1, 0f))
        //     .Append(_draggingHand.transform.DOScale(Vector3.one, .05f).OnComplete(() => { draggingHand.OpenHand(); }))
        //     .SetLoops(-1);
    }

    public static void OnComplete()
    {
        _onComplete?.Invoke();
        Hide();
    }
}