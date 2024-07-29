using UnityEngine;
public class DailyLoginTest : MonoBehaviour
{
    public int day;
    [ContextMenu("ShowDailyLoginPopup")]
    public void ShowDailyLoginPopup()
    {
       // GameInstaller.Instance.SystemLocator.UIManager.Switch(UITypes.DailyLoginPopup,null);
    }
}
