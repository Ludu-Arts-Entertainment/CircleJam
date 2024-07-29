public class AcceptFriendInteractionButton : FriendInteractionButtonBase
{
    public override void Process()
    {
        _friendManager.AcceptFriendRequest(new FriendRequestArguments()
        {
            FriendId = _friendInfoModel.PlatformId,
            FriendIdType = FriendIdType.Custom
        });
    }
}
