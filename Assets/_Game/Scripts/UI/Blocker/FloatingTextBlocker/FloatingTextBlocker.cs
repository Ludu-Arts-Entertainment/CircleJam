using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextBlocker : BlockerBase
{
    [SerializeField] private List<FloatingTextTypeByPoolId> floatingTextTypeByPoolIds = new List<FloatingTextTypeByPoolId> ();
    private Dictionary<FloatingTextType, string> floatingTextTypeToPoolId = new Dictionary<FloatingTextType, string>();

     private void Awake() 
    {
        foreach (var item in floatingTextTypeByPoolIds)
        {
            floatingTextTypeToPoolId.TryAdd(item.floatingTextType, item.poolId);
        }
    }

    private void OnEnable() 
    {
        GameInstaller.Instance.SystemLocator.EventManager.Subscribe<Events.OnFloatingTextAnimationStart>(StartAnimation);
    }

    private void OnDisable() 
    {
        GameInstaller.Instance.SystemLocator.EventManager.Unsubscribe<Events.OnFloatingTextAnimationStart>(StartAnimation);
    }

    private void StartAnimation(Events.OnFloatingTextAnimationStart _event)
    {
        var floatingTextType = floatingTextTypeToPoolId.TryGetValue(_event.floatingTextType, out var value) ? 
            value : UITypes.FloatingTextBlocker;

        var floatingText = GameInstaller.Instance.SystemLocator.PoolManager.Instantiate<FloatingText>(floatingTextType,  parent : transform);
        floatingText.SetText(_event.text);
        floatingText.StartAnimation();
    }
}
public partial class UITypes
{
    public const string FloatingTextBlocker = "FloatingTextBlocker";
}

public partial class Events
{
    public class OnFloatingTextAnimationStart : IEvent
    {
        public FloatingTextType floatingTextType;
        public string text;
        public OnFloatingTextAnimationStart(FloatingTextType floatingTextType, string text)
        {
            this.floatingTextType = floatingTextType;
            this.text = text;
        }
    }
}

public enum FloatingTextType
{
    FromBelow = 0,
    FromAbove = 1,
    HideOut = 2,
    MoveUpAndHide = 3,
}

[Serializable]
public struct FloatingTextTypeByPoolId
{
    public FloatingTextType floatingTextType;
    public string poolId;
}