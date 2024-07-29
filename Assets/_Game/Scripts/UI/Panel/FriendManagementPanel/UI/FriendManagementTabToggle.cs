using System;
using UnityEngine;
using UnityEngine.UI;

public class FriendManagementTabToggle : Toggle
{
    public Action<FriendsTabType, bool> OnTabToggleValueChanged;
    public FriendsTabType friendsTabType;
    public TMPro.TextMeshProUGUI _textMeshProUGUI;
    public float textOffset = 15;
    protected override void OnEnable()
    {
        base.OnEnable();
        onValueChanged.AddListener(OnToggleValueChanged);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        
        onValueChanged.RemoveListener(OnToggleValueChanged);
    }

    private void OnToggleValueChanged(bool value)
    {
        OnTabToggleValueChanged?.Invoke(friendsTabType, value);
        base.targetGraphic.enabled= !value;
        base.graphic.enabled = value;
        if (_textMeshProUGUI)
        {
            _textMeshProUGUI.rectTransform.anchoredPosition = value ? new Vector2(0, -textOffset/2) : new Vector2(0, textOffset/2);
        }
    }
}
public enum FriendsTabType
{
    Friends=0,
    AddFriends=1,
    Inbox=2,
}

