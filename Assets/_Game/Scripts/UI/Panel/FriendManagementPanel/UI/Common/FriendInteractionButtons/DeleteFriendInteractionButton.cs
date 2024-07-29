public class DeleteFriendInteractionButton : FriendInteractionButtonBase
{
    public override void Process()
    {
        _friendManager.DenyFriendRequest(new FriendRequestArguments()
        {
            FriendId = _friendInfoModel.PlatformId,
            FriendIdType = FriendIdType.Custom
        });
    }
}