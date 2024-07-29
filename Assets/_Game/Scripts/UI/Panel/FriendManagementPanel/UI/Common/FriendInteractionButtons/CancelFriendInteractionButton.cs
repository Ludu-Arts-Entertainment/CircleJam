public class CancelFriendInteractionButton : FriendInteractionButtonBase
{
    public override void Process()
    {
        _friendManager.RemoveFriend(new FriendRequestArguments()
        {
            FriendId = _friendInfoModel.PlatformId,
            FriendIdType = FriendIdType.Custom
        });
    }
}
