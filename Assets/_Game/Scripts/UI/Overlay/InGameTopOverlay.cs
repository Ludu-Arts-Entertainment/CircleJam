using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class InGameTopOverlay : PanelBase
{
    [SerializeField] private TextMeshProUGUI moveCountText, goalCountText;

    private NumberAnimator _numberAnimator;

    private void Awake() 
    {
        _numberAnimator = new NumberAnimator(0, goalCountText);
    }

    protected override void OnShown()
    {
        base.OnShown();
        GameInstaller.Instance.SystemLocator.EventManager.Subscribe<Events.MoveCountUpdated>(OnMoveCountUpdated);
        GameInstaller.Instance.SystemLocator.EventManager.Subscribe<Events.GoalUpdated>(OnGoalUpdated);
        GameInstaller.Instance.SystemLocator.EventManager.Subscribe<Events.OnLevelLoaded>(OnLevelLoaded);
    }

    protected override void OnHidden()
    {
        base.OnHidden();
        GameInstaller.Instance.SystemLocator.EventManager.Unsubscribe<Events.MoveCountUpdated>(OnMoveCountUpdated);
        GameInstaller.Instance.SystemLocator.EventManager.Unsubscribe<Events.GoalUpdated>(OnGoalUpdated);
        GameInstaller.Instance.SystemLocator.EventManager.Unsubscribe<Events.OnLevelLoaded>(OnLevelLoaded);
    }

    private void OnLevelLoaded(Events.OnLevelLoaded loaded)
    {
        if(_numberAnimator != null)
            _numberAnimator.StopAnimation();
        
        UpdateValue(GameInstaller.Instance.SystemLocator.GoalManager.CurrentGoalCount, 0.001f);
    }

    private void OnGoalUpdated(Events.GoalUpdated updated)
    {
        if(updated.WithAnimation)
        {
            if (_numberAnimator.Current>GameInstaller.Instance.SystemLocator.GoalManager.CurrentGoalCount)
            {
                goalCountText.transform.DOScale(Vector3.one * 1.5f, 0.1f)
                .SetEase(Ease.OutSine).SetLoops(8, LoopType.Yoyo)
                .OnComplete(() =>
                {
                    goalCountText.transform.DOScale(Vector3.one, 0.2f)
                        .SetEase(Ease.InSine);
                });
            }
            
            if (updated.Amount>0f)
            {
                UpdateValue((float)Convert.ToDecimal(goalCountText.text) - updated.Amount, 0.8f);
            }else
                UpdateValue(GameInstaller.Instance.SystemLocator.GoalManager.CurrentGoalCount);
        }
        else
        {
            UpdateValue(GameInstaller.Instance.SystemLocator.GoalManager.CurrentGoalCount, 0.001f);
        }
    }

    private void OnMoveCountUpdated(Events.MoveCountUpdated updated)
    {
        moveCountText.text = updated.CurrentMoveCount.ToString();
    }

    public void UpdateValue(float to, float duration = 0.8f)=>_numberAnimator.UpdateValue(to, duration, true);
}

public partial class UITypes
{
    public const string InGameTopOverlay = "InGameTopOverlay";
}