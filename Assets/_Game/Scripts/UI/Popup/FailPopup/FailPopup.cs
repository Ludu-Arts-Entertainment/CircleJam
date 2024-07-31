using UnityEngine;
using UnityEngine.UI;

public class FailPopup : PopupBase
{
    [SerializeField] private Button retryButton, closeButton;

    protected override void OnShown()
    {
        base.OnShown();
        retryButton.onClick.AddListener(OnRetryButtonClicked);
        closeButton.onClick.AddListener(OnRetryButtonClicked);
        GameInstaller.Instance.SystemLocator.EventManager.Trigger(new Events.OnLevelStopped());
    }

    protected override void OnHidden()
    {
        base.OnHidden();
        retryButton.onClick.RemoveListener(OnRetryButtonClicked);
        closeButton.onClick.RemoveListener(OnRetryButtonClicked);
        GameInstaller.Instance.SystemLocator.EventManager.Trigger(new Events.OnLevelContiuned());
    }

    private void OnRetryButtonClicked()
    {
        GameInstaller.Instance.SystemLocator.LevelManager.DisposeLevel();
        GameInstaller.Instance.SystemLocator.LevelManager.LoadLevel();
        ClosePanel();
    }

    public void ClosePanel()
    {
        GameInstaller.Instance.SystemLocator.UIManager.Hide(UITypes.FailPopup);
    }
}

public partial class UITypes
{
    public const string FailPopup = "FailPopup";
}