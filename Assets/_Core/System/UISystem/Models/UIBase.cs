using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class UIBase : MonoBehaviour
{
    public Action Shown;
    public Action Hidden;
    private Button _inputBlocker;
    private void Awake()
    {
        _inputBlocker = transform.parent.GetChild(0).GetComponent<Button>();
    }
    public abstract BaseUITypes BaseUIType { get; }
    public abstract void Hide();
    public abstract void Show(IBaseUIData data);
    protected abstract void OnShown();
    protected abstract void OnHidden();
    public virtual void InputBlocker(bool isBlock, UnityAction onBlockerClicked = null)
    {
        _inputBlocker ??= transform.parent.GetChild(0).GetComponent<Button>();
        
        switch (isBlock)
        {
            case true when onBlockerClicked != null:
                _inputBlocker.onClick.AddListener(onBlockerClicked);
                break;
            case false when onBlockerClicked != null:
                _inputBlocker.onClick.RemoveListener(onBlockerClicked);
                break;
        }
        _inputBlocker?.gameObject?.SetActive(isBlock);
    }
}
