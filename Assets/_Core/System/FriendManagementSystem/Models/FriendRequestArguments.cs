public struct FriendRequestArguments
{
    public FriendIdType FriendIdType;
    public string FriendId;
    public FriendInfoModel FriendInfoModel;
}
public enum FriendIdType
{
    Custom,
    Username,
    Email,
    DisplayName
}