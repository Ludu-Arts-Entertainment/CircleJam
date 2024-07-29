#if !FriendManager_Modified
using System;

public class FriendInfoModel
{
    public ProfileSummaryData ProfileSummary { get; set; }
    public UsernameModel DisplayName { get; set; }
    public string PlatformId { get; set; }
    public string AvatarUrl { get; set; }
    public FriendshipStatus FriendshipStatus { get; set; }
    public ButtonType[] Buttons
    {
        get
        {
            return FriendshipStatus switch
            {
                FriendshipStatus.Confirmed => new[] { ButtonType.Gift, ButtonType.Delete},
                FriendshipStatus.Receiver => new[] { ButtonType.Cancel },
                FriendshipStatus.Sender => new[] { ButtonType.Accept, ButtonType.Cancel },
                FriendshipStatus.Facebook => new[] { ButtonType.Gift },
                FriendshipStatus.Self => new[] { ButtonType.Edit },
                FriendshipStatus.None => new[] { ButtonType.Add },
                _ => Array.Empty<ButtonType>()
            };
        }
    }
    [Flags]
    public enum ButtonType
    {
        None = 0,
        Accept = 1<<0,
        Deny = 1<<1,
        Add = 1<<2,
        Cancel = 1<<3,
        Delete = 1<<4,
        Play = 1<<5,
        Gift = 1<<6,
        Claim = 1<<7,
        Edit = 1<<8,
    }
}
#endif