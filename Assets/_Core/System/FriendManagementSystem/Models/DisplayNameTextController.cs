#if !FriendManager_Modified
using TMPro;
using UnityEngine;
public class DisplayNameTextController : MonoBehaviour
{
    [SerializeField] private TMP_Text displayNameText;
    private UsernameModel _displayNameModel;
    public void SetText(UsernameModel displayNameModel,string format = "{0}")
    {
        _displayNameModel = displayNameModel;
        displayNameText.text = string.Format(format, displayNameModel.LongUsername);
    }
    public void CopyText()
    {
        GUIUtility.systemCopyBuffer = _displayNameModel.LongUsername;
        GameInstaller.Instance.SystemLocator.EventManager.Trigger(new Events.OnFloatingTextAnimationStart(FloatingTextType.MoveUpAndHide, "Copied!"));
    }
}
#endif
