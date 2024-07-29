using System;
using DG.Tweening;
using UnityEngine;


public class ClickHandController 
{
    private const string PoolId = "ClickHand";
    private static GameObject _hand;
    private static bool _isLoaded;
    private static Action _onComplete;
    private static Sequence _sequence = DOTween.Sequence();

    private void Awake()
    {
        if (!_isLoaded) LoadHand();
    }

    private static void LoadHand()
    {
        _isLoaded = true;
        //_hand = PoolingSystem.Instance.InstantiateUI(PoolId, Vector2.zero, default);
        _hand.gameObject.SetActive(false);
    }

    public static void Show(Vector3 t, Action callback = null, bool isRect = false, float scale = 1f,
        float handScale = 1f)
    {
        if (!_isLoaded) LoadHand();
        _onComplete = callback;
        _hand.SetActive(true);
        if (isRect)
        {
            _hand.GetComponent<RectTransform>().anchoredPosition = t;
        }
        else
        {
            t.y = t.y + 120;
            _hand.transform.position = t;
            _hand.transform.localScale = Vector3.one;
        }

        _hand.transform.localScale = Vector3.one * scale;
    }


    public static void Hide()
    {
        if (!_isLoaded) LoadHand();
        _sequence.Kill();
        _hand.transform.localScale = Vector3.one;

        _hand.SetActive(false);
    }

    public static void OnComplete()
    {
        _onComplete?.Invoke();
        Hide();
    }
}