using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FriendsTabToggle : Toggle
{
    private CanvasGroup _canvasGroup;
    private CanvasGroup CanvasGroup
    {
        get
        {
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();
            return _canvasGroup;
        }
    }
    public UnityAction<bool> OnValueChangedAction;

    protected override void Awake()
    {
        base.Awake();
        onValueChanged.AddListener(OnValueChanged);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        CanvasGroup.alpha = isOn ? 1 : 0.5f;
    }

    private void OnValueChanged(bool arg0)
    {
        OnValueChangedAction?.Invoke(arg0);
        CanvasGroup.alpha = arg0 ? 1 : 0.5f;
    }
}
