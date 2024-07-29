public class AddFriendInteractionButton : FriendInteractionButtonBase
{
    public override void Process()
    {
        _friendManager.SendFriendRequest(new FriendRequestArguments()
        {
            FriendId = _friendInfoModel.PlatformId,
            FriendIdType = FriendIdType.Custom,
            FriendInfoModel = _friendInfoModel
        }, OnRequestSuccess, OnRequestFail);
        
        void OnRequestSuccess(FriendRequestResponse obj)
        {
            if (obj.success)
            {
                _eventManager.Trigger(new Events.OnFloatingTextAnimationStart(FloatingTextType.MoveUpAndHide,
                    "Friend request sent"));
            }
            else
                _eventManager.Trigger(
                    new Events.OnFloatingTextAnimationStart(FloatingTextType.MoveUpAndHide, obj.message));
        }
        void OnRequestFail(FriendRequestResponse obj)
        {
            _eventManager.Trigger(
                new Events.OnFloatingTextAnimationStart(FloatingTextType.MoveUpAndHide, obj.message));
        }
    }
}
