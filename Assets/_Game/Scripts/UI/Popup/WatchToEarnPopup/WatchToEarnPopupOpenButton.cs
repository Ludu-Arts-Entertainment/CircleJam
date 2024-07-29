using UnityEngine;

public class WatchToEarnPopupOpenButton : MonoBehaviour
{
    public void OpenWatchToEarnPopup()
    {
        GameInstaller.Instance.SystemLocator.UIManager.Show(UITypes.WatchToEarnPopup, null);
    }
}
