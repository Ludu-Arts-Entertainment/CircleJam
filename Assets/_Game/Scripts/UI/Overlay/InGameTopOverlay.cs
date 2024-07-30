using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InGameTopOverlay : PanelBase
{
    [SerializeField] private TextMeshProUGUI moveCountText;

    protected override void OnShown()
    {
        base.OnShown();
        GameInstaller.Instance.SystemLocator.EventManager.Subscribe<Events.MoveCountUpdated>(OnMoveCountUpdated);
    }

    protected override void OnHidden()
    {
        base.OnHidden();
        GameInstaller.Instance.SystemLocator.EventManager.Unsubscribe<Events.MoveCountUpdated>(OnMoveCountUpdated);
    }

    private void OnMoveCountUpdated(Events.MoveCountUpdated updated)
    {
        moveCountText.text = updated.CurrentMoveCount.ToString();
    }
}

public partial class UITypes
{
    public const string InGameTopOverlay = "InGameTopOverlay";
}