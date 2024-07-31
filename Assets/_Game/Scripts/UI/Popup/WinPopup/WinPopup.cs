using UnityEngine;
using UnityEngine.UI;

public class WinPopup : PopupBase
{
    [SerializeField] private Button contiuneButton, closeButton;

    protected override void OnShown()
    {
        base.OnShown();
        contiuneButton.onClick.AddListener(OnContiuneButtonClicked);
        closeButton.onClick.AddListener(OnContiuneButtonClicked);
        GameInstaller.Instance.SystemLocator.EventManager.Trigger(new Events.OnLevelStopped());
    }

    protected override void OnHidden()
    {
        base.OnHidden();
        contiuneButton.onClick.RemoveListener(OnContiuneButtonClicked);
        closeButton.onClick.RemoveListener(OnContiuneButtonClicked);
        GameInstaller.Instance.SystemLocator.EventManager.Trigger(new Events.OnLevelContiuned());
    }

    private void OnContiuneButtonClicked()
    {
        GameInstaller.Instance.SystemLocator.LevelManager.LevelComplete();
        GameInstaller.Instance.SystemLocator.LevelManager.LoadLevel();
        ClosePanel();
    }

    public void ClosePanel()
    {
        GameInstaller.Instance.SystemLocator.UIManager.Hide(UITypes.WinPopup);
    }
}
public partial class UITypes
{
    public const string WinPopup = "WinPopup";
}