using UnityEngine;
using UnityEngine.UI;

public abstract class FriendInteractionButtonBase : MonoBehaviour
{
    protected FriendInfoModel _friendInfoModel;
    protected FriendManager _friendManager;
    protected EventManager _eventManager;
    private Button _button;
    protected Button Button=>_button ? _button : (_button = GetComponent<Button>());
    private void Awake()
    {
        _friendManager = GameInstaller.Instance.SystemLocator.FriendManager;
        _eventManager = GameInstaller.Instance.SystemLocator.EventManager;
        Button.onClick.AddListener(Process);
    }
    public virtual void SetData(FriendInfoModel friendInfoModel)
    {
        _friendInfoModel = friendInfoModel;
    }
    public abstract void Process();
}
