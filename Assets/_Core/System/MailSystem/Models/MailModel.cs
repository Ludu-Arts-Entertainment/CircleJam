using Newtonsoft.Json;

public class MailModel
{
    public string id { get; set; }
    public MailTag Tag { get; set; }
    public MailStatus Status { get; set; }
    public bool IsClaimAndDelete = false;
    public string Title { get; set; }
    public string Message { get; set; }
    public string SenderId { get; set; }
    public string ReceiverId { get; set; }
    public long SentTimestamp { get; set; }
    public long ExpireTimestamp { get; set; }
    public MailAttachmentModel AttachmentModel { get; set; }
}

public enum MailStatus
{
    Unread,
    Read,
    Claimed
}
public enum MailTag
{
    DailyGift = 2,
    System = 4,
}